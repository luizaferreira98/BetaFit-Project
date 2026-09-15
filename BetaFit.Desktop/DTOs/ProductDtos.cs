using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetaFit.Domain.Enums;
namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO para transferência de dados de um Product.
    /// Usado para retornar informações de produtos na API e nas Views.
    /// O preço é demonstrativo: não existe carrinho, checkout ou pagamento real.
    /// </summary>
    /// <remarks>
    /// IMPORTANTE: este DTO precisa espelhar
    /// <c>BetaFit.Application/DTOs/ProductDto.cs</c>. A API faz sobrescrita
    /// direta no PUT /api/products/{id} (ProductService.UpdateAsync), ou seja:
    /// qualquer propriedade que não vier no JSON é gravada com o valor default.
    /// Quando estes DTOs ficam desatualizados, editar um produto pelo Desktop
    /// APAGA silenciosamente tamanhos, cores, SKU, preço de oferta e estoque
    /// que foram cadastrados pela web.
    /// </remarks>
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        /// <summary>Código interno do produto (Stock Keeping Unit).</summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>Preço promocional. Null quando o produto não está em oferta.</summary>
        public decimal? SalePrice { get; set; }

        /// <summary>Estoque total do produto.</summary>
        public int Stock { get; set; }

        /// <summary>Quantidade a partir da qual o estoque é considerado baixo.</summary>
        public int LowStockThreshold { get; set; } = 5;

        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();

        /// <summary>Cores disponíveis para o cliente escolher.</summary>
        public List<string> AvailableColors { get; set; } = new();

        /// <summary>Foto principal de cada cor: { "Preto": "/images/..." }.</summary>
        public Dictionary<string, string> ColorImageUrls { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);

        public Gender Gender { get; set; }
        public int CategoryId { get; set; }

        /// <summary>
        /// Nome da categoria (obtido via JOIN com a tabela Categories).
        /// Evita que o front-end precise fazer uma segunda requisição.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Preço que vale na vitrine: a oferta quando existe, senão o cheio.</summary>
        public decimal EffectivePrice => SalePrice ?? Price;
    }

    /// <summary>
    /// DTO para criação de um novo Product.
    /// Contém apenas os campos que o usuário precisa preencher.
    /// Note que Id e CreatedAt NÃO estão aqui — são gerados automaticamente.
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal? SalePrice { get; set; }
        public int Stock { get; set; } = 999;
        public int LowStockThreshold { get; set; } = 5;
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<string> AvailableColors { get; set; } = new();
        public Dictionary<string, string> ColorImageUrls { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
        public Gender Gender { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO para atualização de um Product existente.
    /// </summary>
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal? SalePrice { get; set; }
        public int Stock { get; set; } = 999;
        public int LowStockThreshold { get; set; } = 5;
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<string> AvailableColors { get; set; } = new();
        public Dictionary<string, string> ColorImageUrls { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
        public Gender Gender { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Opções fixas de variação, espelhando as listas hard-coded da web
    /// (Views/Admin/NewProduct.cshtml). Ficam aqui pra que Desktop e web
    /// ofereçam exatamente os mesmos valores.
    /// </summary>
    public static class ProductVariationOptions
    {
        public static readonly string[] Tamanhos =
        {
            "PP", "P", "M", "G", "GG", "XG", "Único",
            "35", "36", "37", "38", "39", "40",
            "41", "42", "43", "44", "45"
        };

        public static readonly string[] Cores =
        {
            "Preto", "Branco", "Cinza", "Azul",
            "Verde neon", "Rosa", "Vermelho", "Bege"
        };
    }
}