using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaFit.Infraestructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Size).HasMaxLength(30);
            builder.Property(i => i.Color).HasMaxLength(40);

            builder.HasOne(i => i.Product)
                   .WithMany()
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Restrict); // não deleta pedido em cascata ao apagar produto
        }
    }
}
