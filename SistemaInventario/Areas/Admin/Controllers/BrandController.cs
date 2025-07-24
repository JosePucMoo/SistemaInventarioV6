using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Utilities;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IWorkUnit _workUnit;

        public BrandController (IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Brand brand = new Brand();

            if (id == null)
            {
                //Create new brand
                brand.Status = true;
                return View(brand);
            }

            //Update brand
            brand = await _workUnit.Brand.Get(id.GetValueOrDefault());

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Brand brand)
        {
            if (ModelState.IsValid)
            {
                if (brand.Id == 0)
                {
                    await _workUnit.Brand.Add(brand);
                    TempData[DS.Success] = "Marca creada exitosamente";
                }
                else
                {
                    _workUnit.Brand.Update(brand);
                    TempData[DS.Success] = "Marca actualizada exitosamente";
                }

                await _workUnit.Save();

                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar marca";

            return View(brand);
        }

        

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _workUnit.Brand.GetAll();

            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var brandBD = await _workUnit.Brand.Get(id);
            if (brandBD == null)
            {
                return Json(new { success = false, message = "Error al borrar marca, no existe en la base de datos" });
            }

            _workUnit.Brand.Remove(brandBD);
            await _workUnit.Save();

            return Json(new { success = true, message = "Marca borrada exitosamente" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _workUnit.Brand.GetAll();

            if(id == 0)
            {
                value = list.Any(s => s.Name.ToLower().Trim() == name.ToLower().Trim());
            }else
            {
                value = list.Any(s => s.Name.ToLower().Trim() == name.ToLower().Trim() && s.Id != id);
            }

            return Json(new { data = value });
        }
        #endregion
    }
}
