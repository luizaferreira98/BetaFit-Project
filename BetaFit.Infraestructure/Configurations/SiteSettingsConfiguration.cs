using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetaFit.Infraestructure.Configurations;

public sealed class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> b)
    {
        b.ToTable("SiteSettings");
        b.HasKey(x => x.Id);
        b.Property(x => x.Announcement).HasMaxLength(160).IsRequired();
        b.Property(x => x.HeaderLinksJson).HasMaxLength(6000).IsRequired();
        b.Property(x => x.HeroSlidesJson).HasMaxLength(24000).IsRequired();
    }
}
