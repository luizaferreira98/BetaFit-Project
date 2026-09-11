using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetaFit.Infraestructure.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> b)
    {
        b.ToTable("Favorites");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        b.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
        b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
