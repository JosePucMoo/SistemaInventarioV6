using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models.ViewModels
{
    public class KardexInventoryVM
    {
        public Product Product { get; set; }
        public IEnumerable<KardexInventory> KardexInventoryList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
