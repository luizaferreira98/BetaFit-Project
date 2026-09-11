using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetaFit.Infraestructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> b)
    {
        b.ToTable("CartItems");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.Size).HasMaxLength(30);
        b.Property(x => x.Color).HasMaxLength(40);
        b.Property(x => x.Quantity).IsRequired();
        b.HasIndex(x => new { x.UserId, x.ProductId, x.Size, x.Color }).IsUnique();
        b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
