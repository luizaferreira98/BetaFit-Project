using BetaFit.Domain.Enums;

namespace BetaFit.Application.DTOs
{
    public class ProductDto
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
        public string CategoryName { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

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
