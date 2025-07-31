using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventory)]
    public class ProductController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWorkUnit workUnit, IWebHostEnvironment webHostEnvironment)
        {
            _workUnit = workUnit;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _workUnit.Product.GetAllDropdownList("Category"),
                BrandList = _workUnit.Product.GetAllDropdownList("Brand"),
                FatherList = _workUnit.Product.GetAllDropdownList("Product")
            };

            if (id == null)
            {
                productVM.Product.Status = true;
                return View(productVM);
            }
            else
            {
                productVM.Product = await _workUnit.Product.Get(id.GetValueOrDefault());

                if (productVM.Product == null)
                {
                    return NotFound();
                }
            }

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    string upload = webRootPath + DS.ImageRoute;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = fileName + extension;
                    await _workUnit.Product.Add(productVM.Product);
                }
                else
                {
                    var objProduct = await _workUnit.Product.GetFirst(p => p.Id == productVM.Product.Id, isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImageRoute;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var beforeImage = Path.Combine(upload, objProduct.ImageUrl); 
                        if(System.IO.File.Exists(beforeImage))
                        {
                            System.IO.File.Delete(beforeImage);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.ImageUrl = fileName + extension;
                    }
                    else //Caso contrario no se carga nueva imagen
                    {
                        productVM.Product.ImageUrl = objProduct.ImageUrl;
                    }

                    _workUnit.Product.Update(productVM.Product);
                }

                TempData[DS.Success] = "Imagen guardada exitosamente";
                await _workUnit.Save();
                return View("Index");
            }
            productVM.CategoryList = _workUnit.Product.GetAllDropdownList("Category");
            productVM.BrandList = _workUnit.Product.GetAllDropdownList("Brand");
            productVM.FatherList = _workUnit.Product.GetAllDropdownList("Product");
            return View(productVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _workUnit.Product.GetAll(includeProperties: "Category,Brand");

            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productBD = await _workUnit.Product.Get(id);
            if (productBD == null)
            {
                return Json(new { success = false, message = "Error al borrar producto, no existe en la base de datos" });
            }

            string upload = _webHostEnvironment.WebRootPath + DS.ImageRoute;
            var beforeImage = Path.Combine(upload, productBD.ImageUrl);
            if (System.IO.File.Exists(beforeImage))
            {
                System.IO.File.Delete(beforeImage);
            }

            _workUnit.Product.Remove(productBD);
            await _workUnit.Save();

            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateSerialNumber(string serialNumber, int id = 0)
        {
            bool value = false;
            var list = await _workUnit.Product.GetAll();

            if(id == 0)
            {
                value = list.Any(s => s.SerialNumber.ToLower().Trim() == serialNumber.ToLower().Trim());
            }else
            {
                value = list.Any(s => s.SerialNumber.ToLower().Trim() == serialNumber.ToLower().Trim() && s.Id != id);
            }

            return Json(new { data = value });
        }
        #endregion
    }
}
