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
    public class InventoryDetailConfiguration : IEntityTypeConfiguration<InventoryDetail>
    {
        public void Configure(EntityTypeBuilder<InventoryDetail> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.InventoryId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.StockBefore).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.Inventory)
                .WithMany()
                .HasForeignKey(x => x.InventoryId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
