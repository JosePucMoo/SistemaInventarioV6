using SistemaInventario.DataAccess.Repository.IRepository;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Order order)
        {
            _context.Update(order);
        }

        public void UpdatePaymentStripeId(int orderId, string sessionId, string transactionId)
        {
            var orderBD = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (orderBD != null)
            {
                if (!String.IsNullOrEmpty(sessionId))
                {
                    orderBD.SessionId = sessionId;
                }

                if (!String.IsNullOrEmpty(transactionId))
                {
                    orderBD.TransactionId = transactionId;
                    orderBD.PaymentDate = DateTime.Now;
                }
            }
        }

        public void UpdateStatus(int orderId, string orderStatus, string paymentStatus)
        {
            var orderBD = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (orderBD != null)
            {
                orderBD.OrderStatus = orderStatus;
                orderBD.PaymentStatus = paymentStatus;
            }
        }
    }
}
