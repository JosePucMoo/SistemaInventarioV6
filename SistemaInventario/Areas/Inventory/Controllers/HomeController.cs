using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ErrorViewModel;
using SistemaInventario.Models.Specifications;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWorkUnit _workUnit;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IWorkUnit workUnit)
        {
            _logger = logger;
            _workUnit = workUnit;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string search = "", string currentSearch = "")
        {
            // Control session
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                // Add value to session
                var shoppingCartList = await _workUnit.ShoppingCart.GetAll(s => s.UserAppId == claim.Value);
                var totalProducts = shoppingCartList.Count();
                HttpContext.Session.SetInt32(DS.SesionShoppingCart, totalProducts);
            }

            if (!String.IsNullOrEmpty(search))
            {
                pageNumber = 1;
            } else
            {
                search = currentSearch;
            }
            ViewData["CurrentSearch"] = search;

            if (pageNumber < 1) pageNumber = 1;

            Params parameters = new Params()
            {
                PageNumber = pageNumber,
                PageSize = 4
            };

            var result = _workUnit.Product.GetAllPaged(parameters);

            if (!String.IsNullOrEmpty(search))
            {
                result = _workUnit.Product.GetAllPaged(parameters, p => p.Description.Contains(search));
            }

            ViewData["TotalPages"] = result.MetaData.TotalPages;
            ViewData["TotalRegisters"] = result.MetaData.TotalCount;
            ViewData["PageSize"] = result.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previous"] = "disabled";
            ViewData["Next"] = "";

            if (pageNumber > 1) ViewData["Previous"] = "";
            if (result.MetaData.TotalPages <= pageNumber) ViewData["Next"] = "disabled";

            return View(result);
        }

        public async Task<IActionResult> Detail(int productId)
        {
            shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.Company = await _workUnit.Company.GetFirst();
            shoppingCartVM.Product = await _workUnit.Product.GetFirst(p => p.Id == productId, 
                includeProperties: "Brand,Category");
            var storeProduct = await _workUnit.StoreProduct.GetFirst(s => s.ProductId == productId &&
                s.StoreId == shoppingCartVM.Company.StoreSaleId);

            if (storeProduct == null) 
            {
                shoppingCartVM.Stock = 0;
            }
            else
            {
                shoppingCartVM.Stock = storeProduct.Amount;
            }
            shoppingCartVM.ShoppingCart = new ShoppingCart()
            {
                Product = shoppingCartVM.Product,
                ProductId = productId
            };

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detail(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.ShoppingCart.UserAppId = claim.Value;

            ShoppingCart shoppingCartDB = await _workUnit.ShoppingCart.GetFirst( s => s.UserAppId == claim.Value &&
                s.ProductId == shoppingCartVM.ShoppingCart.ProductId);

            if (shoppingCartDB == null)
            {
                await _workUnit.ShoppingCart.Add(shoppingCartVM.ShoppingCart);
            }else
            {
                shoppingCartDB.Amount += shoppingCartVM.ShoppingCart.Amount;
                _workUnit.ShoppingCart.Update(shoppingCartDB);
            }

            await _workUnit.Save();
            TempData[DS.Success] = "Producto agregado al carro de compras";

            // Add value to session
            var shoppingCartList = await _workUnit.ShoppingCart.GetAll(s => s.UserAppId == claim.Value);
            var totalProducts = shoppingCartList.Count();
            HttpContext.Session.SetInt32(DS.SesionShoppingCart, totalProducts);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
