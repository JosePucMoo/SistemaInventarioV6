using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Areas.Inventario.Controllers;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ShoppingCartController : Controller
    {
        private readonly IWorkUnit _workUnit;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public ShoppingCartController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.Order = new Models.Order();
            shoppingCartVM.ShoppingCartList = await _workUnit.ShoppingCart.GetAll(
                s => s.UserAppId == claim.Value, includeProperties: "Product");
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

    }
}
