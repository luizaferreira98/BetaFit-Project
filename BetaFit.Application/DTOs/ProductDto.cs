// =============================================================================
// BetaFit.Application - DTO ProductDto
// =============================================================================
//  CONCEITO IMPORTANTE: DTO (Data Transfer Object)
// Um DTO é um objeto usado para TRANSFERIR dados entre camadas.
// Ele contém apenas os dados necessários, sem lógica de negócio.
//
// Por que usar DTOs ao invés de enviar a Entidade diretamente?
// 1. Segurança: evita expor dados internos do banco
// 2. Flexibilidade: permite enviar apenas os campos necessários
// 3. Desacoplamento: a API não depende da estrutura do banco
// =============================================================================

using BetaFit.Domain.Enums;
using System.Reflection;

namespace BetaFit.Application.DTOs
{
    /// <summary>
    /// DTO para transferência de dados de um Product.
    /// Usado para retornar informações de produtos na API e nas Views.
    /// O preço é demonstrativo: não existe carrinho, checkout ou pagamento real.
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
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
        public Gender Gender { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}