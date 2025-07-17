using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository
{
    public class WorkUnit : IWorkUnit
    {
        private readonly ApplicationDbContext _context;
        public IStoreRepository Store {  get; private set; }

        public WorkUnit(ApplicationDbContext context)
        {
            _context = context;
            Store = new StoreRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
