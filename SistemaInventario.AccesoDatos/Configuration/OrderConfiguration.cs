using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DataAccess.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserAppId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.TotalOrder).IsRequired();
            builder.Property(x => x.OrderStatus).IsRequired();
            builder.Property(x => x.PaymentStatus).IsRequired();
            builder.Property(x => x.ClientName).IsRequired();
            builder.Property(x => x.TrackingNumber).IsRequired(false);
            builder.Property(x => x.Carrier).IsRequired(false);
            builder.Property(x => x.TransactionId).IsRequired(false);
            builder.Property(x => x.SessionId).IsRequired(false);
            builder.Property(x => x.Telephone).IsRequired(false);
            builder.Property(x => x.Address).IsRequired(false);
            builder.Property(x => x.City).IsRequired(false);
            builder.Property(x => x.Country).IsRequired(false);

            builder.HasOne(x => x.UserApp)
                .WithMany()
                .HasForeignKey(x => x.UserAppId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
