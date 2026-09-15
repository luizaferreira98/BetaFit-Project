using System.Text.RegularExpressions;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services;

public class ShippingService(IShippingRuleRepository rules) : IShippingService
{
    public async Task<List<ShippingOptionDto>> QuoteAsync(string? cep, decimal productsAfterDiscount) =>
        Calculate(await rules.GetActiveAsync(), cep, productsAfterDiscount);

    public static List<ShippingOptionDto> Calculate(IEnumerable<ShippingRule> rules, string? cep, decimal productsAfterDiscount)
    {
        if (!Regex.IsMatch(cep ?? "", @"^\d{5}-?\d{3}$")) throw new InvalidOperationException("Informe um CEP com 8 dígitos para calcular o frete.");
        var normalized = cep!.Replace("-", "");
        if (normalized == "00000000") throw new InvalidOperationException("Informe um CEP válido.");
        if (productsAfterDiscount < 0) throw new InvalidOperationException("O valor dos produtos não pode ser negativo.");
        var options = rules.Where(r => r.Active && string.CompareOrdinal(normalized, r.CepStart) >= 0 && string.CompareOrdinal(normalized, r.CepEnd) <= 0)
            .Select(r => new ShippingOptionDto {
                RuleId = r.Id, Name = r.Name, Cost = r.FreeAbove.HasValue && productsAfterDiscount >= r.FreeAbove.Value ? 0 : r.Price,
                RegularCost = r.Price, MinDays = r.MinDays, MaxDays = r.MaxDays, FreeAbove = r.FreeAbove,
                RemainingForFree = r.FreeAbove.HasValue ? Math.Max(0, r.FreeAbove.Value - productsAfterDiscount) : null
            }).OrderBy(o => o.Cost).ThenBy(o => o.MaxDays).ThenBy(o => o.RuleId).ToList();
        if (options.Count == 0) throw new InvalidOperationException("Ainda não há entrega disponível para este CEP. Fale com a loja.");
        return options;
    }
}
