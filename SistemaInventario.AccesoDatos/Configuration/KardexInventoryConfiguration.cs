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
    public class KardexInventoryConfiguration : IEntityTypeConfiguration<KardexInventory>
    {
        public void Configure(EntityTypeBuilder<KardexInventory> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StoreProductId).IsRequired();
            builder.Property(x => x.UserAppId).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Detail).IsRequired();
            builder.Property(x => x.StockBefore).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Cost).IsRequired();
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.RegistrationDate).IsRequired();
            builder.Property(x => x.Total).IsRequired();

            builder.HasOne(x => x.StoreProduct)
                .WithMany()
                .HasForeignKey(x => x.StoreProductId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.UserApp)
                .WithMany()
                .HasForeignKey(x => x.UserAppId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
