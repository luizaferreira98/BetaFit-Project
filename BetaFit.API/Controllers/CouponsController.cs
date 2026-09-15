using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace BetaFit.API.Controllers;
[ApiController,Authorize(Roles="Admin"),Route("api/coupons")]
public class CouponsController(BetaFitDbContext db):ControllerBase {
 [HttpGet] public async Task<IActionResult> Get(){
  await db.DiscountCoupons.Where(x=>x.Active&&(x.ExpiresAt<=DateTime.UtcNow||x.Used>=x.MaxUses)).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.Active,false));
  return Ok(await db.DiscountCoupons.AsNoTracking().OrderByDescending(x=>x.Id).ToListAsync());
 }
 [HttpPost] public async Task<IActionResult> Create(CouponDto dto){
  var code=dto.Code.Trim().ToUpperInvariant();if(dto.ExpiresAt<=DateTime.UtcNow)return BadRequest(new{message="A validade deve ser futura."});
  if(await db.DiscountCoupons.AnyAsync(x=>x.Code==code))return Conflict(new{message="Este código já existe."});
  db.DiscountCoupons.Add(new DiscountCoupon{Code=code,Percent=dto.Percent,Minimum=dto.Minimum,ExpiresAt=DateTime.SpecifyKind(dto.ExpiresAt,DateTimeKind.Utc),MaxUses=dto.MaxUses,Active=dto.Active});
  try{await db.SaveChangesAsync();}catch(DbUpdateException ex) when(ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && sql.Number is 2601 or 2627){return Conflict(new{message="Este código já existe."});}return NoContent();
 }
 [HttpPost("{id:int}/toggle")] public async Task<IActionResult> Toggle(int id){var row=await db.DiscountCoupons.FindAsync(id);if(row==null)return NotFound();if(!row.Active&&(row.ExpiresAt<=DateTime.UtcNow||row.Used>=row.MaxUses))return BadRequest(new{message="Cupom expirado ou esgotado não pode ser reativado."});row.Active=!row.Active;await db.SaveChangesAsync();return NoContent();}
}
