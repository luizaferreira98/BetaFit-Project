

using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetaFit.Infraestructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Total).HasColumnType("decimal(18,2)");
        builder.Property(o => o.PaymentId).HasMaxLength(200);
        builder.Property(o => o.PaymentMethod).HasMaxLength(40).IsRequired();
        builder.Property(o => o.PaymentStatus).HasMaxLength(40).IsRequired();
            builder.Property(o => o.CustomerCpf).HasMaxLength(14);
            builder.Property(o => o.ShippingCep).HasMaxLength(9);
            builder.Property(o => o.ShippingStreet).HasMaxLength(180);
            builder.Property(o => o.ShippingNumber).HasMaxLength(20);
            builder.Property(o => o.ShippingComplement).HasMaxLength(120);
            builder.Property(o => o.ShippingNeighborhood).HasMaxLength(120);
            builder.Property(o => o.ShippingCity).HasMaxLength(120);
            builder.Property(o => o.ShippingState).HasMaxLength(2);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
