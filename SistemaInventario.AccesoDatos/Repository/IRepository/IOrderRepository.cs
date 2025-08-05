using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);

        void UpdateStatus(int orderId, string orderStatus, string paymentStatus);
        void UpdatePaymentStripeId(int orderId, string sessionId, string transactionId);
    }
}
