using BetaFit.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [System.ComponentModel.DataAnnotations.StringLength(80)] public string Sku { get; set; } = "";
        [System.ComponentModel.DataAnnotations.Range(0.01,999999.99)] public decimal? SalePrice { get; set; }
        [System.ComponentModel.DataAnnotations.Range(0,100000)] public int LowStockThreshold { get; set; } = 5;
public List<BetaFit.Domain.Entities.ProductVariant> Variants {get;set;}=new();
    public decimal EffectivePrice => SalePrice ?? Price;
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, List<string>> ColorGalleries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dictionary<string, decimal>> SizeMeasurements { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool MeasurementsAreDemo { get; set; } = true;
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
    public bool IsActive {get;set;}=true;
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Range(0, 999999.99)]
    public decimal Price { get; set; }
    [System.ComponentModel.DataAnnotations.StringLength(80)] public string Sku { get; set; } = "";
        [System.ComponentModel.DataAnnotations.Range(0.01,999999.99)] public decimal? SalePrice { get; set; }
        [System.ComponentModel.DataAnnotations.Range(0,100000)] public int LowStockThreshold { get; set; } = 5;
public List<BetaFit.Domain.Entities.ProductVariant> Variants {get;set;}=new();
    public decimal EffectivePrice => SalePrice ?? Price;
    [Range(0, 100000)]
    public int Stock { get; set; } = 999;
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, List<string>> ColorGalleries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dictionary<string, decimal>> SizeMeasurements { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool MeasurementsAreDemo { get; set; } = true;
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
    [System.ComponentModel.DataAnnotations.StringLength(80)] public string Sku { get; set; } = "";
        [System.ComponentModel.DataAnnotations.Range(0.01,999999.99)] public decimal? SalePrice { get; set; }
        [System.ComponentModel.DataAnnotations.Range(0,100000)] public int LowStockThreshold { get; set; } = 5;
public List<BetaFit.Domain.Entities.ProductVariant> Variants {get;set;}=new();
    public decimal EffectivePrice => SalePrice ?? Price;
    [Range(0, 100000)]
    public int Stock { get; set; } = 999;
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public Dictionary<string, List<string>> ColorGalleries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dictionary<string, decimal>> SizeMeasurements { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool MeasurementsAreDemo { get; set; } = true;
    public Dictionary<string, string> ColorImageUrls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Gender Gender { get; set; }
    public int CategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
}
