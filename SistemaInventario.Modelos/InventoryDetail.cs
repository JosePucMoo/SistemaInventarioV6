using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class InventoryDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int StockBefore { get; set; }
        [Required]
        public int Amount { get; set; }

        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
