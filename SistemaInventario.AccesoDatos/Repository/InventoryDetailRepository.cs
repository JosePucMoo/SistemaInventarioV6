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
    public class InventoryDetailRepository : Repository<InventoryDetail>, IInventoryDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryDetailRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(InventoryDetail inventoryDetail)
        {
            var inventoryDetailBD = _context.InventoryDetails.FirstOrDefault(s => s.Id == inventoryDetail.Id);

            if (inventoryDetailBD != null)
            {
                inventoryDetailBD.StockBefore = inventoryDetail.StockBefore;
                inventoryDetailBD.Amount = inventoryDetail.Amount;

                _context.SaveChanges();
            }
        }

    }
}
