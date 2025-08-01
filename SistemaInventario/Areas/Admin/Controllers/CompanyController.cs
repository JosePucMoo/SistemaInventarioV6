using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IWorkUnit _workUnit;

        public CompanyController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public async Task<IActionResult> Upsert()
        {
            CompanyVM companyVM = new CompanyVM()
            {
                Company = new Models.Company(),
                StoreList = _workUnit.Inventory.GetAllDropDownList("Store")
            };

            companyVM.Company = await _workUnit.Company.GetFirst();

            if (companyVM.Company == null) 
            {
                companyVM.Company = new Models.Company();
            }

            return View(companyVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompanyVM companyVM)
        {
            if (ModelState.IsValid)
            {
                TempData[DS.Success] = "Compañia grabada exitosamente";
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if(companyVM.Company.Id == 0)
                {
                    companyVM.Company.CreatedById = claim.Value;
                    companyVM.Company.UpdatedById = claim.Value;
                    companyVM.Company.CreationDate = DateTime.Now;
                    companyVM.Company.UpdateDate = DateTime.Now;
                    await _workUnit.Company.Add(companyVM.Company);
                    await _workUnit.Save();
                }
                else
                {
                    companyVM.Company.UpdatedById = claim.Value;
                    companyVM.Company.UpdateDate = DateTime.Now;
                    _workUnit.Company.Update(companyVM.Company);
                    await _workUnit.Save();
                    return RedirectToAction("Index", "Home", new {area="Inventory"});
                }
            }
            else
            {
                TempData[DS.Error] = "Error al grabar compañia";
            }

            return View(companyVM);
        }
    }
}
