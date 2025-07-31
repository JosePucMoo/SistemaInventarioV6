using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        void Update(Inventory inventory);

        IEnumerable<SelectListItem> GetAllDropDownList(string obj);

    }
}
