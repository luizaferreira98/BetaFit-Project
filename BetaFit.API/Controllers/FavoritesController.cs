using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController]
[Authorize]
[Route("api/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly BetaFitDbContext _db;
    public FavoritesController(BetaFitDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<int>>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _db.Favorites.AsNoTracking().Where(x => x.UserId == userId).Select(x => x.ProductId).ToListAsync());
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<FavoriteStatusDto>> GetStatus(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(new FavoriteStatusDto { ProductId=productId, IsFavorite=await _db.Favorites.AnyAsync(x=>x.UserId==userId&&x.ProductId==productId) });
    }

    [HttpPost("{productId:int}")]
    public async Task<ActionResult<FavoriteStatusDto>> Add(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!await _db.Products.AnyAsync(p=>p.Id==productId&&p.IsActive)) return NotFound(new { message="Produto não encontrado." });
        if (!await _db.Favorites.AnyAsync(x=>x.UserId==userId&&x.ProductId==productId))
        {
            _db.Favorites.Add(new Domain.Entities.Favorite{UserId=userId,ProductId=productId});
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException) { /* índice único protege concorrência; estado final é favorito */ }
        }
        return Ok(new FavoriteStatusDto{ProductId=productId,IsFavorite=true});
    }

    [HttpDelete("{productId:int}")]
    public async Task<ActionResult<FavoriteStatusDto>> Remove(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _db.Favorites.Where(x=>x.UserId==userId&&x.ProductId==productId).ExecuteDeleteAsync();
        return Ok(new FavoriteStatusDto{ProductId=productId,IsFavorite=false});
    }
}
