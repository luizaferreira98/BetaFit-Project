using System.Text.Json;
using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Route("api/site-settings")]
public sealed class SiteSettingsController : ControllerBase
{
    private readonly BetaFitDbContext _db;
    public SiteSettingsController(BetaFitDbContext db) => _db = db;

    [HttpGet, AllowAnonymous]
    public async Task<SiteSettingsDto> Get()
    {
        var value = await _db.SiteSettings.AsNoTracking().FirstOrDefaultAsync();
        return value is null ? Default() : Map(value);
    }

    [HttpPut, Authorize(Roles="Admin")]
    public async Task<IActionResult> Update([FromBody] SiteSettingsDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        dto.HeaderLinks = dto.HeaderLinks.Where(x => !string.IsNullOrWhiteSpace(x.Label) && !string.IsNullOrWhiteSpace(x.Url)).Take(12).ToList();
        dto.HeroSlides = dto.HeroSlides.Where(x => !string.IsNullOrWhiteSpace(x.Title)).Take(8).ToList();
        var value = await _db.SiteSettings.FirstOrDefaultAsync();
        if (value is null) { value = new SiteSettings(); _db.SiteSettings.Add(value); }
        value.Announcement = string.IsNullOrWhiteSpace(dto.Announcement) ? Default().Announcement : dto.Announcement.Trim();
        value.HeaderLinksJson = JsonSerializer.Serialize(dto.HeaderLinks);
        value.HeroSlidesJson = JsonSerializer.Serialize(dto.HeroSlides);
        value.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(value));
    }

    private static SiteSettingsDto Map(SiteSettings x) => new() { Announcement=x.Announcement, HeaderLinks=Read<List<HeaderLinkDto>>(x.HeaderLinksJson) ?? new(), HeroSlides=Read<List<HeroSlideDto>>(x.HeroSlidesJson) ?? new() };
    private static T? Read<T>(string json) { try { return JsonSerializer.Deserialize<T>(json); } catch { return default; } }
    private static SiteSettingsDto Default() => new() { HeroSlides = new() { new HeroSlideDto { Title="Vista sua disciplina.",Subtitle="Performance, conforto e identidade em cada peça.",ButtonText="Ver coleção",ButtonUrl="/Catalog" } } };
}
