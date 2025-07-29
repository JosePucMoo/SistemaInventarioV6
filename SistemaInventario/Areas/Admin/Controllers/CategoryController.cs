using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Utilities;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IWorkUnit _workUnit;

        public CategoryController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                //Create new category
                category.Status = true;
                return View(category);
            }

            //Update category
            category = await _workUnit.Category.Get(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    await _workUnit.Category.Add(category);
                    TempData[DS.Success] = "Categoria creada exitosamente";
                }
                else
                {
                    _workUnit.Category.Update(category);
                    TempData[DS.Success] = "Categoria actualizada exitosamente";
                }

                await _workUnit.Save();

                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar categoria";

            return View(category);
        }

        

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _workUnit.Category.GetAll();

            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryBD = await _workUnit.Category.Get(id);
            if (categoryBD == null)
            {
                return Json(new { success = false, message = "Error al borrar categoria, no existe en la base de datos" });
            }

            _workUnit.Category.Remove(categoryBD);
            await _workUnit.Save();

            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _workUnit.Category.GetAll();

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
