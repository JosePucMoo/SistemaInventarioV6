using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models.ViewModels
{
    public class InventoryVM
    {
        public Inventory Inventory { get; set; }
        public InventoryDetail InventoryDetail { get; set; }
        public IEnumerable<InventoryDetail> InventoryDetailList { get; set; } = new List<InventoryDetail>();
        public IEnumerable<SelectListItem> StoreList { get; set; }
    }
}
