using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class UserApp : IdentityUser
    {
        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Apellido es requerido")]
        [MaxLength(80)]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Direccion es requerido")]
        [MaxLength(200)]
        public string Address {  get; set; }

        [Required(ErrorMessage = "Ciudad es requerido")]
        [MaxLength(60)]
        public string City { get; set; }

        [Required(ErrorMessage = "Pais es requerido")]
        [MaxLength(60)]
        public string Country { get; set; }

        [NotMapped] // No se guarda en la tabla
        public string Role { get; set; }
    }
}
