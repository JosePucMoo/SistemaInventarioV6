using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreController : Controller
    {
        private readonly IWorkUnit _workUnit;

        public StoreController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Store store = new Store();

            if (id == null)
            {
                //Create new store
                store.Status = true;
                return View(store);
            }

            //Update Store
            store = await _workUnit.Store.Get(id.GetValueOrDefault());

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Store store)
        {
            if (ModelState.IsValid)
            {
                if (store.Id == 0)
                {
                    await _workUnit.Store.Add(store);
                    TempData[DS.Success] = "Bodega creada exitosamente";
                }
                else
                {
                    _workUnit.Store.Update(store);
                    TempData[DS.Success] = "Bodega actualizada exitosamente";
                }

                await _workUnit.Save();

                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar bodega";

            return View(store);
        }

        

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _workUnit.Store.GetAll();

            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var storeBD = await _workUnit.Store.Get(id);
            if (storeBD == null)
            {
                return Json(new { success = false, message = "Error al borrar bodega, no existe en la base de datos" });
            }

            _workUnit.Store.Remove(storeBD);
            await _workUnit.Save();

            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _workUnit.Store.GetAll();

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
