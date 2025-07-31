using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Inventory inventory)
        {
            var inventoryBD = _context.Inventories.FirstOrDefault(s => s.Id == inventory.Id);

            if (inventoryBD != null)
            {
                inventoryBD.StoreId = inventory.StoreId;
                inventoryBD.EndDate = inventory.EndDate;
                inventoryBD.Status = inventory.Status;

                _context.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if(obj == "Store")
            {
                return _context.Stores.Where(s => s.Status).Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
            }

            return null;
        }

    }
}
