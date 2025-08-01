using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserAppId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Amount { get; set; }
        [NotMapped]
        public double Price { get; set; }

        [ForeignKey("UserAppId")]
        public UserApp UserApp { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
