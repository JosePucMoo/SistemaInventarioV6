using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ErrorViewModel;
using SistemaInventario.Models.Specifications;
using System.Diagnostics;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventary")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWorkUnit _workUnit;

        public HomeController(ILogger<HomeController> logger, IWorkUnit workUnit)
        {
            _logger = logger;
            _workUnit = workUnit;
        }

        public IActionResult Index(int pageNumber = 1, string search = "", string currentSearch = "")
        {
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
