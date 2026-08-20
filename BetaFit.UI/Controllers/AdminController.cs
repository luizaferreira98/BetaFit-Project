// =============================================================================
// BetaFit.UI - AdminController
// =============================================================================
//  CONCEITO: [Authorize(Roles = "Admin")]
// Protege todo o controller: só usuários autenticados com a role "Admin"
// (criada pelo SeedData da BetaFit.API) podem gerenciar o catálogo.
// =============================================================================

using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Application.ViewModels;
using BetaFit.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BetaFit.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IDashboardService _dashboardService;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            IDashboardService dashboardService,
            IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _dashboardService = dashboardService;
            _env = env;
        }

        /// <summary>
        /// Painel administrativo — métricas rápidas e grade de produtos.
        /// URL: /Admin
        /// </summary>
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Administração";

            try
            {
                ViewData["Dashboard"] = await _dashboardService.GetSummaryAsync();
            }
            catch (HttpRequestException)
            {
                ViewData["Message"] = "Não foi possível carregar as métricas do dashboard.";
            }

            try
            {
                var products = await _productService.GetAllAsync();
                return View(products.OrderByDescending(p => p.CreatedAt).ToList());
            }
            catch (HttpRequestException)
            {
                ViewData["Message"] = "Não foi possível carregar os produtos.";
                return View(new List<ProductDto>());
            }
        }

        // =====================================================================
        // CRUD DE PRODUTOS
        // =====================================================================

        /// <summary>
        /// Formulário de novo produto.
        /// GET /Admin/NewProduct
        /// </summary>
        [HttpGet("NewProduct")]
        public async Task<IActionResult> NewProduct()
        {
            ViewData["Title"] = "Novo produto";
            var viewModel = new ProductFormViewModel { Categories = await _categoryService.GetAllAsync() };
            return View(viewModel);
        }

        /// <summary>
        /// Salva um novo produto (com upload opcional de imagem).
        /// POST /Admin/NewProduct
        /// </summary>
        [HttpPost("NewProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewProduct(ProductFormViewModel viewModel, Gender gender, IFormFile? image)
        {
            viewModel.Categories = await _categoryService.GetAllAsync();

            var imageUrl = viewModel.ImageUrl;
            if (image is not null && image.Length > 0)
            {
                var savedUrl = await SaveProductImageAsync(image);
                if (savedUrl is null)
                {
                    ModelState.AddModelError(string.Empty, "Use imagens JPG, PNG ou WEBP.");
                    return View(viewModel);
                }
                imageUrl = savedUrl;
            }

            var dto = new CreateProductDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                ImageUrl = imageUrl,
                Gender = gender,
                CategoryId = viewModel.CategoryId,
                IsFeatured = viewModel.IsFeatured
            };

            await _productService.CreateAsync(dto);
            TempData["Success"] = "Produto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Formulário de edição de produto.
        /// GET /Admin/EditProduct/5
        /// </summary>
        [HttpGet("EditProduct/{id:int}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            ViewData["Title"] = "Editar produto";

            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();

            var viewModel = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                Categories = await _categoryService.GetAllAsync()
            };

            ViewData["Gender"] = product.Gender;
            return View(viewModel);
        }

        /// <summary>
        /// Processa a edição de um produto.
        /// POST /Admin/EditProduct/5
        /// </summary>
        [HttpPost("EditProduct/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, ProductFormViewModel viewModel, Gender gender, IFormFile? image)
        {
            viewModel.Categories = await _categoryService.GetAllAsync();

            var imageUrl = viewModel.ImageUrl;
            if (image is not null && image.Length > 0)
            {
                var savedUrl = await SaveProductImageAsync(image);
                if (savedUrl is null)
                {
                    ModelState.AddModelError(string.Empty, "Use imagens JPG, PNG ou WEBP.");
                    return View(viewModel);
                }
                imageUrl = savedUrl;
            }

            var dto = new UpdateProductDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                ImageUrl = imageUrl,
                Gender = gender,
                CategoryId = viewModel.CategoryId,
                IsFeatured = viewModel.IsFeatured,
                IsActive = viewModel.IsActive
            };

            var updated = await _productService.UpdateAsync(id, dto);
            if (updated is null) return NotFound();

            TempData["Success"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Confirmação de exclusão de produto.
        /// GET /Admin/DeleteProduct/5
        /// </summary>
        [HttpGet("DeleteProduct/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ViewData["Title"] = "Excluir produto";
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();
            return View(product);
        }

        /// <summary>
        /// Processa a exclusão de um produto.
        /// POST /Admin/DeleteProduct/5
        /// </summary>
        [HttpPost("DeleteProduct/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Produto excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveProductImageAsync(IFormFile image)
        {
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
                return null;

            var folder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
            await image.CopyToAsync(stream);
            return $"/images/products/{fileName}";
        }
    }
}
