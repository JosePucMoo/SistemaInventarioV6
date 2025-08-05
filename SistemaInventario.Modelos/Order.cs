using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserAppId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        [Required]
        public double TotalOrder { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public string TransactionId { get; set; }
        public string SessionId { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ClientName { get; set; }

        [ForeignKey("UserAppId")]
        public UserApp UserApp { get; set; }
    }
}
