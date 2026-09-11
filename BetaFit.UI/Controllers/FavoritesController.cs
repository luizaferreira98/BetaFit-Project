using BetaFit.Application.Interfaces;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize, Route("Favoritos")]
public class FavoritesController : Controller
{
    private readonly IProductService _products;
    private readonly HttpFavoriteService _favorites;
    public FavoritesController(IProductService products,HttpFavoriteService favorites){_products=products;_favorites=favorites;}
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var ids=await _favorites.GetAsync(); var all=await _products.GetAllAsync();
        return View(all.Where(p=>ids.Contains(p.Id)&&p.IsActive).ToList());
    }
    [HttpPost("Toggle"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id,string? returnUrl=null)
    {
        var isFav=await _favorites.IsFavoriteAsync(id); var ok=isFav?await _favorites.RemoveAsync(id):await _favorites.AddAsync(id);
        TempData["FavoriteMessage"]=ok?(isFav?"Produto removido dos favoritos.":"Produto adicionado aos favoritos."):"Não foi possível atualizar os favoritos.";
        return !string.IsNullOrWhiteSpace(returnUrl)&&Url.IsLocalUrl(returnUrl)?Redirect(returnUrl):RedirectToAction(nameof(Index));
    }
}
