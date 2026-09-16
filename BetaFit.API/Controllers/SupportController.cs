using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BetaFit.API.Controllers;
[ApiController, Authorize, Route("api/support")]
public class SupportController(BetaFitDbContext db) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool Staff => User.IsInRole("Admin") || User.IsInRole("Funcionario");
    private IQueryable<SupportConversation> Accessible => db.SupportConversations.Where(c => Staff || c.UserId == UserId);
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await Accessible.AsNoTracking().OrderByDescending(c => c.UpdatedAt)
        .Select(c => new SupportThreadDto { Id=c.Id,Subject=c.Subject,ProductId=c.ProductId,UpdatedAt=c.UpdatedAt }).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var c=await Accessible.AsNoTracking().FirstOrDefaultAsync(c=>c.Id==id);
        if(c is null) return NotFound();
        var messages=await db.SupportMessages.AsNoTracking().Where(m=>m.ConversationId==id).OrderBy(m=>m.Id)
            .Select(m=>new SupportMessageDto{Id=m.Id,Text=m.Text,IsStaff=m.IsStaff,CreatedAt=m.CreatedAt}).ToListAsync();
        return Ok(new SupportThreadDto{Id=c.Id,Subject=c.Subject,ProductId=c.ProductId,UpdatedAt=c.UpdatedAt,Messages=messages});
    }
    [HttpPost]
    public async Task<IActionResult> Create(SupportCreateDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Text)||dto.Subject.Trim().Length<3) return BadRequest(new{message="Informe o assunto e sua mensagem."});
        if(dto.ProductId.HasValue&&!await db.Products.AnyAsync(p=>p.Id==dto.ProductId)) return BadRequest(new{message="Produto não encontrado."});
        var c=new SupportConversation{UserId=UserId,Subject=dto.Subject.Trim(),ProductId=dto.ProductId};
        c.Messages.Add(new SupportMessage{Text=dto.Text.Trim(),IsStaff=Staff});
        db.SupportConversations.Add(c);await db.SaveChangesAsync();
        return Ok(new SupportThreadDto{Id=c.Id,Subject=c.Subject,ProductId=c.ProductId,UpdatedAt=c.UpdatedAt});
    }
    [HttpPost("{id:int}/messages")]
    public async Task<IActionResult> Send(int id, SupportSendDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Text)) return BadRequest(new{message="Escreva uma mensagem."});
        var c=await Accessible.FirstOrDefaultAsync(c=>c.Id==id);if(c is null)return NotFound();
        c.UpdatedAt=DateTime.UtcNow;
        db.SupportMessages.Add(new SupportMessage{ConversationId=id,Text=dto.Text.Trim(),IsStaff=Staff,CreatedAt=c.UpdatedAt});
        await db.SaveChangesAsync();return NoContent();
    }
}
