using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Areas.Inventario.Controllers;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionService = Stripe.Checkout.SessionService;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ShoppingCartController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly string _webUrl;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public ShoppingCartController(IWorkUnit workUnit, IConfiguration configuration)
        {
            _workUnit = workUnit;
            _webUrl = configuration.GetValue<string>("DomainUrls:WEB_URL");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM
            {
                Order = new Models.Order(),
                ShoppingCartList = await _workUnit.ShoppingCart.GetAll(
                    s => s.UserAppId == claim.Value, includeProperties: "Product")
            };
            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.UserAppId = claim.Value;

            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                item.Price = item.Product.Price;
                shoppingCartVM.Order.TotalOrder += (item.Price * item.Amount);
            }

            return View(shoppingCartVM);
        }

        public async Task<IActionResult> Plus(int shoppingCartId)
        {
            var shoppingCart = await _workUnit.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);
            shoppingCart.Amount++;
            await _workUnit.Save();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Minus(int shoppingCartId)
        {
            var shoppingCart = await _workUnit.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);

            if(shoppingCart.Amount == 1)
            {
                var shoppingCartList = await _workUnit.ShoppingCart.GetAll(
                    s => s.UserAppId == shoppingCart.UserAppId);
                var totalProducts = shoppingCartList.Count();

                _workUnit.ShoppingCart.Remove(shoppingCart);
                HttpContext.Session.SetInt32(DS.SesionShoppingCart, totalProducts - 1);
            }else
            {
                shoppingCart.Amount--; 
            }
            await _workUnit.Save();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int shoppingCartId)
        {
            var shoppingCart = await _workUnit.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);
            var shoppingCartList = await _workUnit.ShoppingCart.GetAll(
                    s => s.UserAppId == shoppingCart.UserAppId);
            var totalProducts = shoppingCartList.Count();

            _workUnit.ShoppingCart.Remove(shoppingCart);
            await _workUnit.Save();

            HttpContext.Session.SetInt32(DS.SesionShoppingCart, totalProducts - 1);
            
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Proceed()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM()
            {
                Order = new Order(),
                ShoppingCartList = await _workUnit.ShoppingCart.GetAll(
                    s => s.UserAppId == claim.Value, includeProperties: "Product"),
                Company = await _workUnit.Company.GetFirst()
            };

            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.UserApp = await _workUnit.UserApp.GetFirst(
                u => u.Id == claim.Value);

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                list.Price = list.Product.Price;
                shoppingCartVM.Order.TotalOrder += (list.Price * list.Amount);
            }

            shoppingCartVM.Order.ClientName = shoppingCartVM.Order.UserApp.Name + " " + shoppingCartVM.Order.UserApp.Lastname;
            shoppingCartVM.Order.Telephone = shoppingCartVM.Order.UserApp.PhoneNumber;
            shoppingCartVM.Order.Address = shoppingCartVM.Order.UserApp.Address;
            shoppingCartVM.Order.Country = shoppingCartVM.Order.UserApp.Country;
            shoppingCartVM.Order.City = shoppingCartVM.Order.UserApp.City;

            // Control stock
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                var stockProduct = await _workUnit.StoreProduct.GetFirst( 
                    sp => sp.ProductId == list.ProductId && sp.StoreId == shoppingCartVM.Company.StoreSaleId);

                if (list.Amount > stockProduct.Amount)
                {
                    TempData[DS.Error] = "La cantidad del producto " 
                        + " " 
                        + list.Product.Description 
                        + " excede al stock actual ("
                        + stockProduct.Amount
                        + " )";

                    return RedirectToAction("Index");
                }
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceed(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.ShoppingCartList = await _workUnit.ShoppingCart.GetAll(
                    s => s.UserAppId == claim.Value, includeProperties: "Product");
            shoppingCartVM.Company = await _workUnit.Company.GetFirst();
            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.UserAppId = claim.Value;
            shoppingCartVM.Order.OrderDate = DateTime.Now;

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                list.Price = list.Product.Price;
                shoppingCartVM.Order.TotalOrder += (list.Price * list.Amount);
            }

            // Control stock
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                var stockProduct = await _workUnit.StoreProduct.GetFirst(
                    sp => sp.ProductId == list.ProductId && sp.StoreId == shoppingCartVM.Company.StoreSaleId);

                if (list.Amount > stockProduct.Amount)
                {
                    TempData[DS.Error] = "La cantidad del producto "
                        + " "
                        + list.Product.Description
                        + " excede al stock actual ("
                        + stockProduct.Amount
                        + " )";

                    return RedirectToAction("Index");
                }
            }

            shoppingCartVM.Order.OrderStatus = DS.PendingState;
            shoppingCartVM.Order.PaymentStatus = DS.PaymentStatusPending;

            await _workUnit.Order.Add(shoppingCartVM.Order);
            await _workUnit.Save();

            //Save detail
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    Product = list.Product,
                    OrderId = shoppingCartVM.Order.Id,
                    Price = list.Price,
                    Amount = list.Amount,
                };
                
                await _workUnit.OrderDetail.Add(orderDetail);
                await _workUnit.Save();
            }

            var user = await _workUnit.UserApp.GetFirst(u => u.Id == claim.Value);
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"inventory/ShoppingCart/ConfirmationOrder?orderId={shoppingCartVM.Order.Id}",
                CancelUrl = _webUrl + "inventory/ShoppingCart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = user.Email,
                Locale = "es"
            };

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(list.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = list.Product.Description
                        }
                    },
                    Quantity = list.Amount
                };

                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            _workUnit.Order.UpdatePaymentStripeId(shoppingCartVM.Order.Id, session.Id ,session.PaymentIntentId);
            await _workUnit.Save();
            Response.Headers.Append("Location", session.Url); //Redirect to Stripe

            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> ConfirmationOrder( int orderId)
        {
            var order = await _workUnit.Order.GetFirst(
                o => o.Id == orderId, includeProperties: "UserApp");
            var service = new SessionService();
            Session session = await service.GetAsync(order.SessionId);

            var shoppingCart = await _workUnit.ShoppingCart.GetAll(
                sc => sc.UserAppId == order.UserAppId);

            if (string.Equals(session.PaymentStatus.ToLower(), "paid"))
            {
                _workUnit.Order.UpdatePaymentStripeId(orderId, session.Id, session.PaymentIntentId);
                _workUnit.Order.UpdateStatus(orderId, DS.ApprovedState, DS.PaymentStatusApproved);
                await _workUnit.Save();

                //Reduce stock 
                var company = await _workUnit.Company.GetFirst();
                foreach (var list in shoppingCart)
                {
                    var storeProduct = await _workUnit.StoreProduct.GetFirst(
                        sp => sp.ProductId == list.ProductId &&
                        sp.StoreId == company.StoreSaleId);

                    await _workUnit.KardexInventory.RegisterKardex(
                        storeProduct.Id, "output", "Venta - Orden #" + orderId,
                        storeProduct.Amount, list.Amount, order.UserAppId);

                    storeProduct.Amount -= list.Amount;
                    await _workUnit.Save();
                }
            }

            //Delete shoppingcart and session of shoppingcart
            List<ShoppingCart> shoppingCartList = shoppingCart.ToList();

            _workUnit.ShoppingCart.RemoveRange(shoppingCartList);
            await _workUnit.Save();

            HttpContext.Session.SetInt32(DS.SesionShoppingCart, 0);

            return View(orderId);
        }
    }
}
