using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserAppId { get; set; }
        [Required]
        public DateTime StarDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage="La bodega es obligatoria")]
        public int StoreId { get; set; }
        [Required]
        public bool Status {  get; set; }

        [ForeignKey("UserAppId")]
        public UserApp UserApp { get; set; }
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
    }
}
