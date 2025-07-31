using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class KardexInventory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StoreProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Type { get; set; }
        [Required]
        public string Detail { get; set; }
        [Required]
        public int StockBefore { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int Stock { get; set; }
        public double Total {  get; set; }
        [Required]
        public string UserAppId { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }


        [ForeignKey("StoreProductId")]
        public StoreProduct StoreProduct { get; set; }
        [ForeignKey("UserAppId")]
        public UserApp UserApp { get; set; }
    }
}
