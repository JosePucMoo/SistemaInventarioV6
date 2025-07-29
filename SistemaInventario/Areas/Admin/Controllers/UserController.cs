using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataAccess;
using SistemaInventario.DataAccess.Repository.IRepository;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly ApplicationDbContext _context;

        public UserController(IWorkUnit workUnit,ApplicationDbContext contex)
        {
            _workUnit = workUnit;
            _context = contex;
        }
        public IActionResult Index()
        {


            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userList = await _workUnit.UserApp.GetAll();
            var userRole = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }

            return Json(new {data = userList});
        }

        [HttpPost]
        public async Task<IActionResult> BlockUnblock([FromBody] string id)
        {
            var user = await _workUnit.UserApp.GetFirst(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "Error de usuario" });
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now) 
            { 
                user.LockoutEnd = DateTime.Now;
            } else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            await _workUnit.Save();

            return Json(new { success = true, message = "Operación exitosa" });
        }

        #endregion
    }
}
