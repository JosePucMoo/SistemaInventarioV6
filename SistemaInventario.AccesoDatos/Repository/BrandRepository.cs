using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Modelos;
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
            var storeBD = _context.Brands.FirstOrDefault(s => s.Id == brand.Id);

            if (storeBD != null)
            {
                storeBD.Name = brand.Name;
                storeBD.Description = brand.Description;
                storeBD.Status = brand.Status;
                _context.SaveChanges();
            }
        }
    }
}
