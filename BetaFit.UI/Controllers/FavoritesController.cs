using BetaFit.Application.Interfaces;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Authorize, Route("Favoritos")]
    public class FavoritesController : Controller
    {
        private readonly IProductService _products;
        public FavoritesController(IProductService products)=>_products=products;
        [HttpGet("")] public async Task<IActionResult> Index(){var ids=FavoriteService.Get(HttpContext);var all=await _products.GetAllAsync();return View(all.Where(p=>ids.Contains(p.Id)&&p.IsActive).ToList());}
        [HttpPost("Toggle"),ValidateAntiForgeryToken] public IActionResult Toggle(int id,string? returnUrl=null){var added=FavoriteService.Toggle(HttpContext,id);TempData["FavoriteMessage"]=added?"Produto adicionado aos favoritos.":"Produto removido dos favoritos.";return !string.IsNullOrWhiteSpace(returnUrl)&&Url.IsLocalUrl(returnUrl)?Redirect(returnUrl):RedirectToAction(nameof(Index));}
    }
}
