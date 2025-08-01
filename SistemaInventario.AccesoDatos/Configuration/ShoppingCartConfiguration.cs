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
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserAppId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.UserApp)
                .WithMany()
                .HasForeignKey(x => x.UserAppId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
