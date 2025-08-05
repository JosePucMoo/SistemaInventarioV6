using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilities
{
    public static class DS
    {
        public const string Success = "Exitosa";
        public const string Error = "Error";

        public const string ImageRoute = @"\images\product\";
        public const string SesionShoppingCart = "Sesion carro compras";

        // Roles usuarios
        public const string Role_Admin = "Admin";
        public const string Role_Client = "Cliente";
        public const string Role_Inventory = "Inventario";

        // Estados de la orden
        public const string PendingState = "Pendiente";
        public const string ApprovedState = "Aprobado";
        public const string InProcessState = "En proceso";
        public const string SentState = "Enviado";
        public const string CanceledState = "Cancelado";
        public const string ReturnedState = "Devuelto";

        // Estado del pago de la orden
        public const string PaymentStatusPending = "Pendiente";
        public const string PaymentStatusApproved = "Aprobado";
        public const string PaymentStatusDelayed = "Retrasado";
        public const string PaymentStatusRejected = "Rechazado";
    }
}
