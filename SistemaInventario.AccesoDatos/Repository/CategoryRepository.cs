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
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Store store)
        {
            var storeBD = _context.Stores.FirstOrDefault(s => s.Id == store.Id);

            if (storeBD != null)
            {
                storeBD.Name = store.Name;
                storeBD.Description = store.Description;
                storeBD.Status = store.Status;
                _context.SaveChanges();
            }
        }
    }
}
