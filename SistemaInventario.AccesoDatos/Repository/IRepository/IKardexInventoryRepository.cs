using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IKardexInventoryRepository : IRepository<KardexInventory>
    {
        Task RegisterKardex(int storeProductId, string type, string detail, int stockBefore, int amount, string userId);
    }
}
