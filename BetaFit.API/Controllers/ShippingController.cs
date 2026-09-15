using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Route("api/shipping")]
public class ShippingController(BetaFitDbContext db, IShippingService shipping, ICouponRepository coupons) : ControllerBase
{
    [HttpPost("quote"), AllowAnonymous, ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Quote(ShippingQuoteRequest dto)
    {
        var ids = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await db.Products.AsNoTracking().Where(p => ids.Contains(p.Id) && p.IsActive).ToDictionaryAsync(p => p.Id);
        if (products.Count != ids.Count) return BadRequest(new { message = "Um produto não está mais disponível. Atualize o carrinho." });
        var quote = new ShippingQuoteDto { Subtotal = dto.Items.Sum(i => (products[i.ProductId].SalePrice ?? products[i.ProductId].Price) * i.Quantity) };
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) quote.CouponError = "Entre na sua conta para aplicar um cupom.";
            else try { var coupon = await coupons.PreviewAsync(dto.CouponCode, userId, quote.Subtotal); quote.CouponCode = coupon.Code; quote.Discount = coupon.Discount; }
            catch (InvalidOperationException ex) { quote.CouponError = ex.Message; }
        }
        try { quote.Options = await shipping.QuoteAsync(dto.Cep, quote.Subtotal - quote.Discount); }
        catch (InvalidOperationException ex) { quote.ShippingError = ex.Message; }
        return Ok(quote);
    }

    [HttpGet("rules"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Rules() => Ok(await db.ShippingRules.AsNoTracking().OrderBy(r => r.CepStart).ThenBy(r => r.Price).ToListAsync());

    [HttpPost("rules"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ShippingRuleDto dto)
    {
        var rule = new ShippingRule(); Apply(rule, dto); db.ShippingRules.Add(rule); await db.SaveChangesAsync(); return Ok(rule);
    }

    [HttpPut("rules/{id:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, ShippingRuleDto dto)
    {
        var rule = await db.ShippingRules.FindAsync(id); if (rule is null) return NotFound();
        Apply(rule, dto); await db.SaveChangesAsync(); return Ok(rule);
    }
    private static void Apply(ShippingRule rule, ShippingRuleDto dto)
    {
        rule.Name = dto.Name.Trim(); rule.CepStart = dto.CepStart.Replace("-", ""); rule.CepEnd = dto.CepEnd.Replace("-", "");
        rule.Price = dto.Price; rule.MinDays = dto.MinDays; rule.MaxDays = dto.MaxDays; rule.FreeAbove = dto.FreeAbove; rule.Active = dto.Active;
    }
}
