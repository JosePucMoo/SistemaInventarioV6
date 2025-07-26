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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Brand brand)
        {
            var brandBD = _context.Brands.FirstOrDefault(s => s.Id == brand.Id);

            if (brandBD != null)
            {
                brandBD.Name = brand.Name;
                brandBD.Description = brand.Description;
                brandBD.Status = brand.Status;
                _context.SaveChanges();
            }
        }
    }
}
