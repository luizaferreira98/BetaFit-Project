using BetaFit.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, string> ColorImageUrls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Gender Gender { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Range(0, 999999.99)]
    public decimal Price { get; set; }
    [Range(0, 100000)]
    public int Stock { get; set; } = 999;
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, string> ColorImageUrls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Gender Gender { get; set; }
    public int CategoryId { get; set; }
    public bool IsFeatured { get; set; }
}

public class UpdateProductDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Range(0, 999999.99)]
    public decimal Price { get; set; }
    [Range(0, 100000)]
    public int Stock { get; set; } = 999;
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, string> ColorImageUrls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Gender Gender { get; set; }
    public int CategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
}
