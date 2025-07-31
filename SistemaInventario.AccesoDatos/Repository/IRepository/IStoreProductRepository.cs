using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IStoreProductRepository : IRepository<StoreProduct>
    {
        void Update(StoreProduct storeProduct);

    }
}
