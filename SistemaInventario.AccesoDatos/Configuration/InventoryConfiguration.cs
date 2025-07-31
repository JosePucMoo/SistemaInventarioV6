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
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StoreId).IsRequired();
            builder.Property(x => x.UserAppId).IsRequired();
            builder.Property(x => x.StarDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            builder.HasOne(x => x.Store)
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.UserApp)
                .WithMany()
                .HasForeignKey(x => x.UserAppId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
