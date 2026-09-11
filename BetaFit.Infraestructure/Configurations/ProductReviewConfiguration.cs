using BetaFit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BetaFit.Infraestructure.Configurations;
public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{ public void Configure(EntityTypeBuilder<ProductReview> b){ b.ToTable("ProductReviews"); b.HasKey(x=>x.Id); b.Property(x=>x.UserId).HasMaxLength(450).IsRequired(); b.Property(x=>x.Comment).HasMaxLength(1000); b.Property(x=>x.PhotoUrlsJson).HasMaxLength(12000).IsRequired(); b.HasIndex(x=>new{x.OrderId,x.ProductId}).IsUnique(); } }
