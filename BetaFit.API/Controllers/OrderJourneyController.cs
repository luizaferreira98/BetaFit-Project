using BetaFit.Application.DTOs;
using BetaFit.Application.Services;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace BetaFit.API.Controllers;
[ApiController,Authorize,Route("api/orders"),BetaFit.API.Services.OrderTransaction]
public class OrderJourneyController(BetaFitDbContext db):ControllerBase {
 string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
 bool Staff=>User.IsInRole("Admin")||User.IsInRole("Estoquista")||User.IsInRole("Funcionario");
 [HttpPost("{id:int}/received")]public async Task<IActionResult> Received(int id){var o=await db.Orders.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==UserId);if(o==null)return NotFound();if(o.Status!=OrderStatus.Enviado)return BadRequest(new{message="Confirme apenas pedidos a caminho."});o.Status=OrderStatus.Entregue;o.DeliveredAt=DateTime.Now;o.TrackingDescription="Recebimento confirmado pelo cliente.";await db.SaveChangesAsync();return NoContent();}
 [HttpPost("{id:int}/refund")]public async Task<IActionResult> Refund(int id){var o=await db.Orders.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==UserId);if(o==null)return NotFound();if(o.Status!=OrderStatus.Entregue)return BadRequest(new{message="A solicitação está disponível após a entrega."});o.Status=OrderStatus.Reembolso;o.TrackingDescription="Solicitação de reembolso em análise. Nenhum estorno real foi processado.";await db.SaveChangesAsync();return NoContent();}
 [HttpPost("{id:int}/experience")]public async Task<IActionResult> Experience(int id,OrderExperienceDto dto){var error=ReviewPolicy.Validate(dto.Comment);if(error!=null)return BadRequest(new{message=error});var o=await db.Orders.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==UserId);if(o==null)return NotFound();if(o.Status!=OrderStatus.Entregue||o.ExperienceRating!=null)return BadRequest(new{message="Experiência já avaliada ou pedido ainda não entregue."});o.ExperienceRating=dto.Rating;o.ExperienceComment=dto.Comment.Trim();await db.SaveChangesAsync();return NoContent();}
 [HttpPost("{id:int}/tracking"),Authorize(Roles="Admin,Funcionario,Estoquista")]public async Task<IActionResult> Tracking(int id,TrackingDto dto){var o=await db.Orders.FindAsync(id);if(o==null)return NotFound();if(o.Status is OrderStatus.Cancelado or OrderStatus.Reembolso or OrderStatus.Entregue)return BadRequest(new{message="Este pedido não aceita atualização de rastreio."});o.TrackingCode=dto.Code.Trim();o.TrackingDescription=dto.Description.Trim();await db.SaveChangesAsync();return NoContent();}
 [HttpGet("{id:int}/messages")]public async Task<IActionResult> Messages(int id){if(!await db.Orders.AnyAsync(o=>o.Id==id&&(o.UserId==UserId||Staff)))return NotFound();return Ok(await db.OrderMessages.Where(m=>m.OrderId==id).OrderBy(m=>m.Id).Select(m=>new OrderMessageDto{Text=m.Text,IsStaff=m.IsStaff,CreatedAt=m.CreatedAt}).ToListAsync());}
 [HttpPost("{id:int}/messages")]public async Task<IActionResult> Message(int id,OrderExperienceDto dto){if(!await db.Orders.AnyAsync(o=>o.Id==id&&(o.UserId==UserId||Staff)))return NotFound();var error=ReviewPolicy.Validate(dto.Comment);if(error!=null)return BadRequest(new{message=error});db.OrderMessages.Add(new OrderMessage{OrderId=id,UserId=UserId,IsStaff=Staff,Text=dto.Comment.Trim()});await db.SaveChangesAsync();return NoContent();}
}
