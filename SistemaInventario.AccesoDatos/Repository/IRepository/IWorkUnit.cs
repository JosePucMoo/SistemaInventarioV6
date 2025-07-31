using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IWorkUnit : IDisposable
    {
        IStoreRepository Store { get; }
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        IUserAppRepository UserApp { get; }
        IStoreProductRepository StoreProduct { get; }
        IInventoryRepository Inventory { get; }
        IInventoryDetailRepository InventoryDetail { get; }
        IKardexInventoryRepository KardexInventory { get; }
        Task Save();
    }
}
