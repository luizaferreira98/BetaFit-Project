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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly HttpReviewService _reviewService;
        private readonly HttpCartService _cartService;
        private readonly HttpFavoriteService _favoriteService;

        public CatalogController(IProductService productService, ICategoryService categoryService, HttpReviewService reviewService, HttpCartService cartService, HttpFavoriteService favoriteService)
        {
            _productService = productService; _categoryService = categoryService; _reviewService = reviewService; _cartService = cartService; _favoriteService = favoriteService;
        }

        /// <summary>
        /// Catálogo de produtos com busca, filtro por categoria/gênero e ordenação.
        /// URL: /Catalog
        /// </summary>
        [HttpGet("Catalog")]
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, Gender? gender, string? availability, string? sortBy, string viewMode = "grid", int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 100) searchTerm = searchTerm[..100];
            ViewData["Title"] = "Catálogo";

            var viewModel = new CatalogViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Gender = gender,
                SortBy = sortBy,
                Availability = availability,
                ViewMode = viewMode == "list" ? "list" : "grid",
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

                query = availability switch { "in" => query.Where(p => p.Stock > 0), "out" => query.Where(p => p.Stock <= 0), _ => query };

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
            ViewData["Reviews"] = product is null ? Array.Empty<ReviewDto>() : await _reviewService.GetByProductAsync(id);
            ViewData["IsFavorite"] = false;
            if (User.Identity?.IsAuthenticated == true && product is not null)
            {
                try { ViewData["IsFavorite"] = await _favoriteService.IsFavoriteAsync(id); } catch (HttpRequestException) { }
            }
            return View(product);
        }

        /// <summary>
        /// Adiciona um produto ao carrinho (Session) e volta para o carrinho.
        /// POST /Catalog/AddToCart
        /// </summary>
        [HttpPost("Catalog/AddToCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1, string? size = null, string? color = null)
        {
            quantity = Math.Clamp(quantity, 1, 99);

            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product is null || !product.IsActive)
                    return NotFound();

                var requiresSize = product.AvailableSizes.Any();
                if (requiresSize && string.IsNullOrWhiteSpace(size))
                {
                    TempData["Error"] = "Selecione um tamanho antes de adicionar este produto.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                if (!requiresSize)
                    size = null;

                if (requiresSize && !product.AvailableSizes.Contains(size!, StringComparer.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "O tamanho selecionado não está disponível para este produto.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                if (product.AvailableColors.Any() && string.IsNullOrWhiteSpace(color))
                {
                    TempData["Error"] = "Selecione uma cor antes de adicionar este produto.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                if (product.AvailableColors.Any() && !product.AvailableColors.Contains(color!, StringComparer.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "A cor selecionada não está disponível para este produto.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                if (!product.AvailableColors.Any()) color = null;

                var added = await _cartService.AddAsync(product.Id, quantity, size, color);
                if (!added.Ok) { TempData["Error"] = added.Message; return RedirectToAction(nameof(Details), new { id }); }
                return RedirectToAction("Index", "Cart");
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Não foi possível adicionar o produto ao carrinho. Tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
