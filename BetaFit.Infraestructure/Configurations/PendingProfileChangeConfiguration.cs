using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BetaFit.Infraestructure.Configurations;
public class PendingProfileChangeConfiguration : IEntityTypeConfiguration<PendingProfileChange>
{ public void Configure(EntityTypeBuilder<PendingProfileChange> b){ b.ToTable("PendingProfileChanges"); b.HasKey(x=>x.Id); b.Property(x=>x.TokenHash).HasMaxLength(128).IsRequired(); b.HasIndex(x=>x.TokenHash).IsUnique(); b.Property(x=>x.UserId).HasMaxLength(450).IsRequired(); b.Property(x=>x.Email).HasMaxLength(256).IsRequired(); b.Property(x=>x.FullName).HasMaxLength(120).IsRequired(); b.Property(x=>x.PhoneNumber).HasMaxLength(30).IsRequired(); b.Property(x=>x.Cpf).HasMaxLength(14); b.Property(x=>x.Cep).HasMaxLength(9); b.Property(x=>x.Street).HasMaxLength(180); b.Property(x=>x.Number).HasMaxLength(20); b.Property(x=>x.Complement).HasMaxLength(120); b.Property(x=>x.Neighborhood).HasMaxLength(120); b.Property(x=>x.City).HasMaxLength(120); b.Property(x=>x.State).HasMaxLength(2); } }
