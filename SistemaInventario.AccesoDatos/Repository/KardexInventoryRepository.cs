using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository
{
    public class KardexInventoryRepository : Repository<KardexInventory>, IKardexInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public KardexInventoryRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task RegisterKardex(int storeProductId, string type, string detail, int stockBefore, int amount, string userId)
        {
            var storeProduct = await _context.StoreProducts.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == storeProductId);

            if(type == "input")
            {
                KardexInventory kardexInventory = new KardexInventory();
                kardexInventory.StoreProductId = storeProductId;
                kardexInventory.Type = type;
                kardexInventory.Detail = detail;
                kardexInventory.StockBefore = stockBefore;
                kardexInventory.Amount = amount;
                kardexInventory.Cost = storeProduct.Product.Cost;
                kardexInventory.Stock = stockBefore + amount;
                kardexInventory.Total = kardexInventory.Stock * kardexInventory.Cost;
                kardexInventory.UserAppId = userId;
                kardexInventory.RegistrationDate = DateTime.Now;

                await _context.AddAsync(kardexInventory);
                await _context.SaveChangesAsync();
            }

            if (type == "output")
            {
                KardexInventory kardexInventory = new KardexInventory();
                kardexInventory.StoreProductId = storeProductId;
                kardexInventory.Type = type;
                kardexInventory.Detail = detail;
                kardexInventory.StockBefore = stockBefore;
                kardexInventory.Amount = amount;
                kardexInventory.Cost = storeProduct.Product.Cost;
                kardexInventory.Stock = stockBefore - amount;
                kardexInventory.Total = kardexInventory.Stock * kardexInventory.Cost;
                kardexInventory.UserAppId = userId;
                kardexInventory.RegistrationDate = DateTime.Now;

                await _context.AddAsync(kardexInventory);
                await _context.SaveChangesAsync();
            }
        }

    }
}
