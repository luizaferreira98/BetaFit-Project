// =============================================================================
// BetaFit.UI - HomeController
// =============================================================================
//  CONCEITO: Controller MVC
// Um Controller recebe requisições HTTP e retorna Views (páginas HTML).
// Cada método público (Action) corresponde a uma URL.
// Exemplo: HomeController.Index()  ->  URL: /  ou  /Home/Index
// =============================================================================

using BetaFit.Application.Interfaces;
using BetaFit.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BetaFit.UI.Services;

namespace BetaFit.UI.Controllers
{
    /// <summary>
    /// Página inicial da loja Beta Fit. Área pública.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly HttpSiteSettingsService _siteSettings;

        public HomeController(IProductService productService, ICategoryService categoryService, HttpSiteSettingsService siteSettings)
        {
            _productService = productService;
            _categoryService = categoryService;
            _siteSettings = siteSettings;
        }

        /// <summary>
        /// Home — produtos em destaque e categorias.
        /// URL: / ou /Home/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Home";

            var viewModel = new HomeViewModel();
            var apiUnavailable = false;

            try
            {
                var featured = await _productService.GetFeaturedAsync();
                viewModel.FeaturedProducts = featured.Where(p => p.IsActive).Take(8).ToList();

                var categories = await _categoryService.GetAllAsync();
                viewModel.Categories = categories.Where(c => c.IsActive).ToList();
                var products = (await _productService.GetAllAsync()).Where(p => p.IsActive).ToList();
                viewModel.RecentProducts = products.OrderByDescending(p => p.CreatedAt).Take(12).ToList();
                viewModel.ProductsByCategory = products.GroupBy(p => p.CategoryName).Where(g => !string.IsNullOrWhiteSpace(g.Key)).ToDictionary(g => g.Key, g => (IReadOnlyList<BetaFit.Application.DTOs.ProductDto>)g.Take(12).ToList());
                viewModel.SiteSettings = await _siteSettings.GetAsync();
            }
            catch (HttpRequestException)
            {
                apiUnavailable = true;
            }

            ViewData["ApiUnavailable"] = apiUnavailable;
            return View(viewModel);
        }

        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
