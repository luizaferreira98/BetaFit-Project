using System.Text.Json;
using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Authorize, Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly BetaFitDbContext _db;
    private readonly UserManager<IdentityUser> _users;
    public ReviewsController(BetaFitDbContext db, UserManager<IdentityUser> users){_db=db;_users=users;}

    [HttpGet("product/{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var rows=await _db.ProductReviews.Where(x=>x.ProductId==productId).OrderByDescending(x=>x.CreatedAt).Take(50).ToListAsync();
        var result=new List<ReviewDto>(); foreach(var row in rows) result.Add(await MapAsync(row)); return Ok(result);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var userId=_users.GetUserId(User)!;
        var isStaff=User.IsInRole("Admin")||User.IsInRole("Funcionario");
        var query=_db.ProductReviews.Where(x=>x.OrderId==orderId);
        if(!isStaff)query=query.Where(x=>x.UserId==userId);
        var rows=await query.OrderByDescending(x=>x.CreatedAt).ToListAsync();
        var result = new List<ReviewDto>(); foreach (var row in rows) result.Add(await MapAsync(row)); return Ok(result);
    }

    [HttpPost("order/{orderId:int}/product/{productId:int}")]
    public async Task<IActionResult> Create(int orderId,int productId,CreateReviewDto dto)
    {
        if(!ModelState.IsValid)return ValidationProblem(ModelState);
        var userId=_users.GetUserId(User)!;
        var order=await _db.Orders.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id==orderId && x.UserId==userId);
        if(order is null)return Forbid();
        if (order.Status != Domain.Enums.OrderStatus.Entregue) return BadRequest(new { message = "A avaliação só pode ser enviada após a entrega do pedido." });
        if(!order.Items.Any(x=>x.ProductId==productId))return BadRequest(new{message="Este produto não pertence ao pedido informado."});
        if(await _db.ProductReviews.AnyAsync(x=>x.OrderId==orderId&&x.ProductId==productId&&x.UserId==userId))return Conflict(new{message="Você já avaliou este produto neste pedido."});
        var review=new ProductReview{OrderId=orderId,ProductId=productId,UserId=userId,Rating=dto.Rating,Comment=dto.Comment?.Trim()??string.Empty,PhotoUrlsJson=JsonSerializer.Serialize((dto.PhotoUrls??new()).Take(5).ToList()),CreatedAt=DateTime.UtcNow};
        _db.ProductReviews.Add(review);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Esta avaliação já foi registrada para este produto neste pedido." });
        }
        return Ok(await MapAsync(review));
    }
    private async Task<ReviewDto> MapAsync(ProductReview x)
    {
        var user = await _users.FindByIdAsync(x.UserId);
        var claims = user is null ? Array.Empty<System.Security.Claims.Claim>() : await _users.GetClaimsAsync(user);
        var displayName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
        return new ReviewDto
        {
            Id=x.Id, OrderId=x.OrderId, ProductId=x.ProductId,
            UserName=string.IsNullOrWhiteSpace(displayName) ? "Cliente" : displayName,
            Rating=x.Rating, Comment=x.Comment,
            PhotoUrls=JsonSerializer.Deserialize<List<string>>(x.PhotoUrlsJson)??new(), CreatedAt=x.CreatedAt
        };
    }
}
