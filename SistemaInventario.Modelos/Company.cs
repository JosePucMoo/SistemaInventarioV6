using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre es requerido")]
        [MaxLength(80)]
        public string Name { get; set; }
        [Required(ErrorMessage = "La descripcion es requerido")]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required(ErrorMessage = "El país es requerido")]
        [MaxLength(60)]
        public string Country { get; set; }
        [Required(ErrorMessage = "La ciudad es requerida")]
        [MaxLength(60)]
        public string City { get; set; }
        [Required(ErrorMessage = "La dirección es requerido")]
        [MaxLength(100)]
        public string Address { get; set; }
        [Required(ErrorMessage = "El teléfono es requerido")]
        [MaxLength(40)]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "La bodega de venta es requerida")]
        public int StoreSaleId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }


        [ForeignKey("StoreSaleId")]
        public Store Store {  get; set; }
        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public UserApp CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public UserApp UpdatedBy { get; set; }

    }
}
