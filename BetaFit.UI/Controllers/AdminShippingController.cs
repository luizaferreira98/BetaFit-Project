using System.Globalization;
using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize(Roles = "Admin"), Route("Admin/Shipping")]
public class AdminShippingController(IHttpClientFactory factory, ILogger<AdminShippingController> logger) : Controller
{
    private HttpClient Api => factory.CreateClient("ApiClient");
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try { return View(await Api.GetFromJsonAsync<List<ShippingRuleDto>>("api/shipping/rules") ?? new()); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        { logger.LogError(ex, "Falha ao carregar fretes"); ViewData["Error"] = "Não foi possível carregar as regras de frete. Tente novamente."; return View(new List<ShippingRuleDto>()); }
    }
    [HttpGet("New")]
    public IActionResult New() => View("Edit", new ShippingRuleDto());
    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        try {
            var rules = await Api.GetFromJsonAsync<List<ShippingRuleDto>>("api/shipping/rules");
            var rule = rules?.FirstOrDefault(r => r.Id == id); return rule is null ? NotFound() : View(rule);
        }
        catch (HttpRequestException) { TempData["Error"] = "Não foi possível carregar a regra."; return RedirectToAction(nameof(Index)); }
    }
    [HttpPost("Save"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ShippingRuleDto dto)
    {
        // Inputs type=number enviam ponto decimal mesmo com a interface pt-BR.
        foreach (var field in new[] { nameof(dto.Price), nameof(dto.FreeAbove) })
        {
            ModelState.Remove(field); var text = Request.Form[field].ToString();
            if (field == nameof(dto.FreeAbove) && string.IsNullOrWhiteSpace(text)) { dto.FreeAbove = null; continue; }
            if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
                ModelState.AddModelError(field, "Informe um valor válido.");
            else if (field == nameof(dto.Price)) dto.Price = value; else dto.FreeAbove = value;
        }
        dto.Name = dto.Name?.Trim() ?? "";
        TryValidateModel(dto);
        if (!ModelState.IsValid) return View("Edit", dto);
        try
        {
            using var response = dto.Id == 0 ? await Api.PostAsJsonAsync("api/shipping/rules", dto) : await Api.PutAsJsonAsync($"api/shipping/rules/{dto.Id}", dto);
            if (!response.IsSuccessStatusCode) { ModelState.AddModelError("", "Não foi possível salvar. Confira os valores, a faixa de CEP e os prazos."); return View("Edit", dto); }
            TempData["Success"] = "Regra de frete salva. Novas cotações já usarão esses valores.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        { logger.LogError(ex, "Falha ao salvar frete"); ModelState.AddModelError("", "API indisponível. Seus dados foram mantidos; tente novamente."); return View("Edit", dto); }
    }
}
