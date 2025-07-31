using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Migrations;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize(Roles = DS.Role_Admin + "," +  DS.Role_Inventory)]
    public class InventoryController : Controller
    {
        private readonly IWorkUnit _workUnit;

        [BindProperty]
        public InventoryVM inventoryVM { get; set; }

        public InventoryController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;   
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewInventory()
        {
            inventoryVM = new InventoryVM()
            {
                Inventory = new Models.Inventory(),
                StoreList = _workUnit.Inventory.GetAllDropDownList("Store")
            };

            inventoryVM.Inventory.Status = false;

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            inventoryVM.Inventory.UserAppId = claim.Value;
            inventoryVM.Inventory.StarDate = DateTime.Now;
            inventoryVM.Inventory.EndDate = DateTime.Now;

            return View(inventoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewInventory(InventoryVM inventoryVM)
        {
            if (ModelState.IsValid)
            {
                inventoryVM.Inventory.StarDate = DateTime.Now;
                inventoryVM.Inventory.EndDate = DateTime.Now;
                await _workUnit.Inventory.Add(inventoryVM.Inventory);
                await _workUnit.Save();
                return RedirectToAction("InventoryDetails", new {id = inventoryVM.Inventory.Id});
            }
            inventoryVM.StoreList = _workUnit.Inventory.GetAllDropDownList("Store");

            return View(inventoryVM);
        }

        public async Task<IActionResult> InventoryDetails(int id)
        {
            inventoryVM = new InventoryVM();
            inventoryVM.Inventory = await _workUnit.Inventory.GetFirst(i => i.Id == id, includeProperties:"Store");
            inventoryVM.InventoryDetailList = await _workUnit.InventoryDetail.GetAll(d => d.InventoryId == id,
                includeProperties:"Product,Product.Brand");

            return View(inventoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InventoryDetails(int inventoryId, int productId, int amountId)
        {
            inventoryVM = new InventoryVM();
            inventoryVM.Inventory = await _workUnit.Inventory.GetFirst(i => i.Id == inventoryId);
            var storeProduct = await _workUnit.StoreProduct.GetFirst(
                sp => sp.ProductId == productId && sp.StoreId == inventoryVM.Inventory.StoreId
            );

            var detail = await _workUnit.InventoryDetail.GetFirst(
                d => d.InventoryId == inventoryId && d.ProductId == productId
            );

            if (detail == null) 
            {
                inventoryVM.InventoryDetail = new InventoryDetail();
                inventoryVM.InventoryDetail.ProductId = productId;
                inventoryVM.InventoryDetail.InventoryId = inventoryId;

                if (storeProduct != null)
                {
                    inventoryVM.InventoryDetail.StockBefore = storeProduct.Amount;
                }
                else
                {
                    inventoryVM.InventoryDetail.StockBefore = 0;
                }

                inventoryVM.InventoryDetail.Amount = amountId;
                await _workUnit.InventoryDetail.Add(inventoryVM.InventoryDetail);
                await _workUnit.Save();
            }
            else
            {
                detail.Amount += amountId;
                await _workUnit.Save();
            }
            return RedirectToAction("InventoryDetails", new {id = inventoryId});
        }

        public async Task<IActionResult> Plus(int detailId)
        {
            inventoryVM = new InventoryVM();
            var detail = await _workUnit.InventoryDetail.Get(detailId);
            inventoryVM.Inventory = await _workUnit.Inventory.Get(detail.InventoryId);
            detail.Amount++;
            await _workUnit.Save();
            return RedirectToAction("InventoryDetails", new {id = detail.InventoryId });
        }

        public async Task<IActionResult> Minus(int detailId)
        {
            inventoryVM = new InventoryVM();
            var detail = await _workUnit.InventoryDetail.Get(detailId);
            inventoryVM.Inventory = await _workUnit.Inventory.Get(detail.InventoryId);

            if (detail.Amount == 1)
            {
                _workUnit.InventoryDetail.Remove(detail);
                _workUnit.Save();
            }
            else
            {
                detail.Amount--;
                await _workUnit.Save();
            }
               
            return RedirectToAction("InventoryDetails", new { id = detail.InventoryId });
        }

        public async Task<IActionResult> GenerateStock(int inventoryId)
        {
            var inventory = await _workUnit.Inventory.Get(inventoryId);
            var detailList = await _workUnit.InventoryDetail.GetAll(d => d.InventoryId == inventoryId);
            //Obtener Id del usuario desde la sesion
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var detail in detailList) 
            { 
                var storeProduct = await _workUnit.StoreProduct.GetFirst(
                    s => s.Id == detail.ProductId && s.StoreId == inventory.StoreId
                );

                if(storeProduct != null)
                {
                    await _workUnit.KardexInventory.RegisterKardex(
                        storeProduct.Id,
                        "input",
                        "Registro de inventario",
                        storeProduct.Amount,
                        detail.Amount,
                        claim.Value
                        );
                    storeProduct.Amount += detail.Amount;
                    await _workUnit.Save();
                }
                else
                {
                    storeProduct = new StoreProduct();
                    storeProduct.StoreId = inventory.StoreId;
                    storeProduct.ProductId = detail.ProductId;
                    storeProduct.Amount = detail.Amount;
                    await _workUnit.StoreProduct.Add(storeProduct);
                    await _workUnit.Save();
                    await _workUnit.KardexInventory.RegisterKardex(
                        storeProduct.Id,
                        "input",
                        "Inicial inventario",
                        0,
                        detail.Amount,
                        claim.Value
                        );
                }

            }
            inventory.Status = true;
            inventory.EndDate = DateTime.Now;
            await _workUnit.Save();
            TempData[DS.Success] = "Stock generado con éxito";
            return RedirectToAction("Index");
        }

        public IActionResult KardexProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProduct(string starDateId, string endDateId, int productId)
        {
            return RedirectToAction("KardexProductResult", new { starDateId, endDateId, productId });
        }
        
        public async Task<IActionResult> KardexProductResult(string starDateId, string endDateId, int productId, DateTime dateTime)
        {
            KardexInventoryVM kardexInventoryVM = new KardexInventoryVM();
            kardexInventoryVM.Product = new Product();
            kardexInventoryVM.Product = await _workUnit.Product.Get(productId);

            kardexInventoryVM.StartDate = DateTime.Parse(starDateId); //00:00:00
            kardexInventoryVM.EndDate = DateTime.Parse(endDateId).AddHours(23).AddHours(59);

            kardexInventoryVM.KardexInventoryList = await _workUnit.KardexInventory.GetAll(
                k => k.StoreProduct.ProductId == productId &&
                (k.RegistrationDate >= kardexInventoryVM.StartDate &&
                k.RegistrationDate <= kardexInventoryVM.EndDate),
                includeProperties: "StoreProduct,StoreProduct.Product,StoreProduct.Store",
                orderBy: o => o.OrderBy(o => o.RegistrationDate)
                );

            return View(kardexInventoryVM);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _workUnit.StoreProduct.GetAll(includeProperties: "Store,Product");
            return Json(new {data = all});
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string term)
        {
            if(!String.IsNullOrEmpty(term))
            {
                var listProduct = await _workUnit.Product.GetAll(p => p.Status);
                var data = listProduct.Where(
                    x => x.SerialNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                return Ok(data);
            }

            return Ok();
        }

        #endregion
    }
}
