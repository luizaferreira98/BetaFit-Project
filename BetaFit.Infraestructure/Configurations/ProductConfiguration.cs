// =============================================================================
// BetaFit.Infraestructure - Configuração da entidade Product (Fluent API)
// =============================================================================
//  CONCEITO: IEntityTypeConfiguration<T>
// Esta classe define as regras de mapeamento da entidade Product para o banco.
// Usando Fluent API, podemos definir:
// - Tamanho máximo de campos (MaxLength)
// - Campos obrigatórios (IsRequired)
// - Precisão de campos decimais (preço)
// - Relacionamentos entre tabelas
// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetaFit.Domain.Entities;

namespace BetaFit.Infraestructure.Configurations
{
    /// <summary>
    /// Configuração Fluent API da entidade Product.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Define a chave primária
            builder.HasKey(p => p.Id);

            // Configurações dos campos
            builder.Property(p => p.Name)
                .IsRequired()           // Campo obrigatório
                .HasMaxLength(200);     // Máximo de 200 caracteres

            builder.Property(p => p.Description)
                .HasMaxLength(2000);    // Máximo de 2000 caracteres

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.ImageUrlsJson)
                .HasMaxLength(8000);


            builder.Property(p => p.Stock)
                .HasDefaultValue(999)
                .IsRequired();
            builder.Property(p => p.AvailableSizesJson)
                .HasMaxLength(1000);
            builder.Property(p => p.AvailableColorsJson)
                .HasMaxLength(1000);
            builder.Property(p => p.ColorImageUrlsJson)
                .HasMaxLength(12000);

            // Preço com precisão decimal explícita (evita warning do EF Core)
            builder.Property(p => p.Price)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.Gender)
                .IsRequired();

            // =====================================================================
            //  CONCEITO: Configuração de Relacionamento (Fluent API)
            // Um Product pertence a UMA Category (relação N:1).
            // Uma Category possui MUITOS Products (relação 1:N).
            // HasOne  WithMany  HasForeignKey
            // =====================================================================
            builder.HasOne(p => p.Category)        // Um Product tem UMA Category
                .WithMany(c => c.Products)          // Uma Category tem MUITOS Products
                .HasForeignKey(p => p.CategoryId)   // A FK é CategoryId
                .OnDelete(DeleteBehavior.Restrict);  // Não permite deletar categoria com produtos
        }
    }
}
