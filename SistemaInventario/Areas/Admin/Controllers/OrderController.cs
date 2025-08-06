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
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IWorkUnit _workUnit;
        [BindProperty]
        public OrderDetailVM orderDetailVM { get; set; }

        public OrderController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            orderDetailVM = new OrderDetailVM()
            {
                Order = await _workUnit.Order.GetFirst(
                    o => o.Id == id, includeProperties: "UserApp"),
                OrderDetailList = await _workUnit.OrderDetail.GetAll(
                    d => d.OrderId == id, includeProperties: "Product")
            };
            return View(orderDetailVM);
        }

        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> Process(int orderId)
        {
            var order = await _workUnit.Order.GetFirst(o => o.Id == orderId);
            order.OrderStatus = DS.InProcessState;
            await _workUnit.Save();
            TempData[DS.Success] = "Orden cambiada a estado en Proceso";
            return RedirectToAction("Detail", new { id = orderId });
        }

        [HttpPost]
        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> SendOrder(OrderDetailVM orderDetailVM)
        {
            var order = await _workUnit.Order.GetFirst(o => o.Id == orderDetailVM.Order.Id);
            order.OrderStatus = DS.SentState;
            order.Carrier = orderDetailVM.Order.Carrier;
            order.TrackingNumber = orderDetailVM.Order.TrackingNumber;
            order.ShippingDate = DateTime.Now;
            await _workUnit.Save();
            TempData[DS.Success] = "Orden cambiada a estado Enviado";
            return RedirectToAction("Detail", new { id = orderDetailVM.Order.Id });
        }


        #region
        [HttpGet]
        public async Task<IActionResult> GetOrderList(string status)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Order> all;
            if (User.IsInRole(DS.Role_Admin))  // Validar el rol del usuario
            {
                all = await _workUnit.Order.GetAll(o => o.OrderStatus != DS.PendingState, includeProperties: "UserApp");
            }
            else
            {
                all = await _workUnit.Order.GetAll(o => o.UserAppId == claim.Value && o.OrderStatus != DS.PendingState, includeProperties: "UserApp");
            }
            // Validar el estado
            switch (status)
            {
                case "aprobado":
                    all = all.Where(o => o.OrderStatus == DS.ApprovedState);
                    break;
                case "completado":
                    all = all.Where(o => o.OrderStatus == DS.SentState);
                    break;
                default:
                    break;
            }

            return Json(new { data = all });
        }
        #endregion

    }
}
