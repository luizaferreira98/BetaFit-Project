using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetaFit.Infraestructure.Configurations;

public sealed class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> b)
    {
        b.ToTable("UserNotifications");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.Title).HasMaxLength(120).IsRequired();
        b.Property(x => x.Message).HasMaxLength(500).IsRequired();
        b.Property(x => x.LinkUrl).HasMaxLength(500);
        b.Property(x => x.Type).HasMaxLength(30).IsRequired();
        b.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
    }
}
