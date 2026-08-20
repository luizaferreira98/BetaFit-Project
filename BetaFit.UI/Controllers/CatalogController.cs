// =============================================================================
// BetaFit.UI - CatalogController (Área Pública)
// =============================================================================
// Controller público do catálogo de roupas — listagem com filtros e a
// página de detalhes de cada produto. NÃO requer autenticação.
//
// Mantém as mesmas URLs do front-end original (BetaFit-master):
//   /Catalog                -> listagem com filtros
//   /Product/{id}            -> detalhes do produto
//   POST /Catalog/AddToCart  -> adiciona ao carrinho (Session)
// =============================================================================

using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Enums;
using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CatalogController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Catálogo de produtos com busca, filtro por categoria/gênero e ordenação.
        /// URL: /Catalog
        /// </summary>
        [HttpGet("Catalog")]
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, Gender? gender, string? sortBy, int page = 1)
        {
            ViewData["Title"] = "Catálogo";

            var viewModel = new CatalogViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Gender = gender,
                SortBy = sortBy,
                Page = page <= 0 ? 1 : page
            };
            var apiUnavailable = false;

            try
            {
                var categories = await _categoryService.GetAllAsync();
                viewModel.Categories = categories.Where(c => c.IsActive).ToList();

                var products = categoryId.HasValue
                    ? await _productService.GetByCategoryAsync(categoryId.Value)
                    : await _productService.GetAllAsync();

                var query = products.Where(p => p.IsActive);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p =>
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (gender.HasValue)
                {
                    query = query.Where(p => p.Gender == gender.Value);
                }

                query = viewModel.SortBy switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };

                var all = query.ToList();
                viewModel.TotalCount = all.Count;
                viewModel.TotalPages = (int)Math.Ceiling(all.Count / (double)viewModel.PageSize);
                viewModel.Items = all
                    .Skip((viewModel.Page - 1) * viewModel.PageSize)
                    .Take(viewModel.PageSize)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                apiUnavailable = true;
            }

            ViewData["ApiUnavailable"] = apiUnavailable;
            return View(viewModel);
        }

        /// <summary>
        /// Detalhes de um produto do catálogo.
        /// URL: /Product/{id}
        /// </summary>
        [HttpGet("Product/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var apiUnavailable = false;
            ProductDto? product = null;
            IReadOnlyList<ProductDto> related = Array.Empty<ProductDto>();

            try
            {
                product = await _productService.GetByIdAsync(id);
                if (product is null || !product.IsActive)
                    return NotFound();

                var sameCategory = await _productService.GetByCategoryAsync(product.CategoryId);
                related = sameCategory.Where(p => p.Id != id && p.IsActive).Take(4).ToList();
            }
            catch (HttpRequestException)
            {
                apiUnavailable = true;
            }

            ViewData["Title"] = product?.Name ?? "Produto";
            ViewData["ApiUnavailable"] = apiUnavailable;
            ViewData["RelatedProducts"] = related;
            return View(product);
        }

        /// <summary>
        /// Adiciona um produto ao carrinho (Session) e volta para o carrinho.
        /// POST /Catalog/AddToCart
        /// </summary>
        [HttpPost("Catalog/AddToCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is not null)
            {
                CartService.Add(HttpContext, product);
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
