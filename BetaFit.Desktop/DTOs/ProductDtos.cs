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
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
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
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public Gender Gender { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um Product existente.
    /// </summary>
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public Gender Gender { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}
