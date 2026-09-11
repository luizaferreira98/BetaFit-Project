using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController]
[Authorize]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly BetaFitDbContext _db;
    public CartController(BetaFitDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = await _db.CartItems.AsNoTracking().Include(x => x.Product).ThenInclude(p => p!.Images)
            .Where(x => x.UserId == userId && x.Product != null && x.Product.IsActive)
            .OrderBy(x => x.Id).ToListAsync();
        return Ok(items.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<CartItemDto>> Add([FromBody] AddCartItemDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
        if (product is null) return NotFound(new { message = "Produto não encontrado." });
        if (product.Stock <= 0) return BadRequest(new { message = "Este produto está esgotado." });
        var quantity = Math.Clamp(dto.Quantity, 1, 99);
        var size = NormalizeAndValidateSize(product.AvailableSizesJson, dto.Size, product.Name);
        if (size == InvalidSize) return BadRequest(new { message = "Tamanho inválido ou obrigatório para este produto." });
        var color = NormalizeAndValidateColor(product.AvailableColorsJson, dto.Color);
        if (color == InvalidSize) return BadRequest(new { message = "Selecione uma cor disponível para este produto." });

        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == product.Id && x.Size == size && x.Color == color);
        if (item is null)
        {
            item = new Domain.Entities.CartItem { UserId = userId, ProductId = product.Id, Size = size, Color = color, Quantity = quantity };
            _db.CartItems.Add(item);
        }
        else item.Quantity = Math.Clamp(item.Quantity + quantity, 1, 99);
        if (item.Quantity > Available(product,size,color)) return BadRequest(new { message = $"Estoque insuficiente. Disponível: {product.Stock}." });
        await _db.SaveChangesAsync();
        return Ok(new CartItemDto { ProductId=product.Id, Name=product.Name, Price=product.SalePrice??product.Price, ImageUrl=GetImage(product), Size=item.Size, Color=item.Color, Quantity=item.Quantity });
    }

    [HttpPut("{productId:int}")]
    public async Task<IActionResult> Update(int productId, [FromBody] AddCartItemDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
        if (product is null) return NotFound(new { message = "Produto não encontrado." });
        if (dto.Quantity > Available(product,dto.Size,dto.Color)) return BadRequest(new { message = $"Estoque insuficiente. Disponível: {product.Stock}." });
        var size = NormalizeAndValidateSize(product.AvailableSizesJson, dto.Size, product.Name);
        if (size == InvalidSize) return BadRequest(new { message = "Tamanho inválido ou obrigatório para este produto." });
        var color = NormalizeAndValidateColor(product.AvailableColorsJson, dto.Color);
        if (color == InvalidSize) return BadRequest(new { message = "Cor inválida ou obrigatória para este produto." });
        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId && x.Size == size && x.Color == color);
        if (item is null) return NotFound(new { message = "Item não encontrado no carrinho." });
        if (dto.Quantity <= 0) _db.CartItems.Remove(item); else item.Quantity = Math.Clamp(dto.Quantity,1,99);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId, [FromQuery] string? size, [FromQuery] string? color)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId && x.Size == (string.IsNullOrWhiteSpace(size) ? null : size.Trim()) && x.Color == (string.IsNullOrWhiteSpace(color) ? null : color.Trim()));
        if (item is null) return NotFound();
        _db.CartItems.Remove(item); await _db.SaveChangesAsync(); return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _db.CartItems.Where(x => x.UserId == userId).ExecuteDeleteAsync();
        return NoContent();
    }

    private static int Available(Domain.Entities.Product p,string? size,string? color){var variants=Domain.Entities.VariantInventory.Read(p);return variants.Count==0?p.Stock:Domain.Entities.VariantInventory.Find(variants,size,color)?.Stock??0;}
    private static readonly string InvalidSize = "\u0000";
    private static string? NormalizeAndValidateSize(string json, string? requested, string productName)
    {
        var sizes = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json ?? "[]") ?? new();
        if (sizes.Count == 0) return null;
        if (string.IsNullOrWhiteSpace(requested)) return InvalidSize;
        var found = sizes.FirstOrDefault(x => string.Equals(x.Trim(), requested.Trim(), StringComparison.OrdinalIgnoreCase));
        return found ?? InvalidSize;
    }
    private static string? NormalizeAndValidateColor(string json, string? requested)
    {
        var colors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json ?? "[]") ?? new();
        if (colors.Count == 0) return null;
        if (string.IsNullOrWhiteSpace(requested)) return InvalidSize;
        return colors.FirstOrDefault(x => string.Equals(x.Trim(), requested.Trim(), StringComparison.OrdinalIgnoreCase)) ?? InvalidSize;
    }
    private static string? GetImage(Domain.Entities.Product p) => p.Images.OrderBy(x => x.SortOrder).Select(x => x.Url).FirstOrDefault() ?? p.ImageUrl;
    private static CartItemDto Map(Domain.Entities.CartItem x) => new() { ProductId=x.ProductId, Name=x.Product!.Name, Price=x.Product.SalePrice??x.Product.Price, ImageUrl=GetImage(x.Product), Size=x.Size, Color=x.Color, Quantity=x.Quantity };
}
