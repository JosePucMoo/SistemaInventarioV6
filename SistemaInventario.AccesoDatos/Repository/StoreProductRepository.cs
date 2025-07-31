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
    public class StoreProductRepository : Repository<StoreProduct>, IStoreProductRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreProductRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(StoreProduct storeProduct)
        {
            var storeProductBD = _context.StoreProducts.FirstOrDefault(s => s.Id == storeProduct.Id);

            if (storeProductBD != null)
            {
                storeProductBD.Amount = storeProduct.Amount;

                _context.SaveChanges();
            }
        }

    }
}
