using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BetaFit.API.Controllers;
[ApiController,Authorize(Roles="Admin,Funcionario,Estoquista"),Route("api/management")]
public class ManagementController(BetaFitDbContext db):ControllerBase {
 [HttpPost("categories/reorder")] public async Task<IActionResult> Reorder(List<int> ids){var rows=await db.Categories.ToListAsync();if(ids.Count!=rows.Count||ids.Distinct().Count()!=ids.Count||ids.Any(id=>rows.All(c=>c.Id!=id)))return BadRequest(new{message="A lista mudou. Recarregue antes de ordenar."});for(int i=0;i<ids.Count;i++)rows.First(c=>c.Id==ids[i]).SortOrder=i;await db.SaveChangesAsync();return NoContent();}
 [HttpPost("categories/{id:int}/toggle")] public async Task<IActionResult> Toggle(int id,[FromBody] bool active){var c=await db.Categories.FindAsync(id);if(c==null)return NotFound();c.IsActive=active;await db.SaveChangesAsync();return NoContent();}
 [HttpPost("products/{id:int}/quick")] public async Task<IActionResult> Quick(int id,QuickProductDto dto){var p=await db.Products.FindAsync(id);if(p==null)return NotFound();if(dto.Price.HasValue){if(p.SalePrice>=dto.Price)return BadRequest(new{message="O preço original deve superar a oferta."});p.Price=dto.Price.Value;}if(dto.Stock.HasValue){if(VariantInventory.Read(p).Count>0)return BadRequest(new{message="Edite o estoque por variação no cadastro do produto."});p.Stock=dto.Stock.Value;}await db.SaveChangesAsync();return NoContent();}
}
public class QuickProductDto{[System.ComponentModel.DataAnnotations.Range(0,999999.99)]public decimal? Price{get;set;}[System.ComponentModel.DataAnnotations.Range(0,100000)]public int? Stock{get;set;}}
