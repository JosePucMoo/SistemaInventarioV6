using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numero de serie es requerido")]
        [MaxLength(60)]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Descripcion es requerido")]
        [MaxLength(100, ErrorMessage = "Descripcion debe ser máximo 100 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Precio es requerido")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Costo es requerido")]
        public double Cost { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Estado requerido")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Categoria requerida")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Marca requerida")]
        public int BrandId { get; set; }



        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public int? FatherId { get; set; }
        public virtual Product Father { get; set; }

    }
}
