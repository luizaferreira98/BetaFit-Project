using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
}

public class AddCartItemDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }
    [Range(1, 99)]
    public int Quantity { get; set; }
    [StringLength(30)]
    public string? Size { get; set; }
    [StringLength(40)]
    public string? Color { get; set; }
}
