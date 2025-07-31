using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IInventoryDetailRepository : IRepository<InventoryDetail>
    {
        void Update(InventoryDetail inventoryDetail);

    }
}
