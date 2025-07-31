using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class StoreProduct
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Amount { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
