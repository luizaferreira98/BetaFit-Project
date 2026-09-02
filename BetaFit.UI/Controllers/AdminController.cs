using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Application.ViewModels;
using BetaFit.Domain.Enums;
using BetaFit.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Authorize(Roles="Admin"), Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService; private readonly ICategoryService _categoryService; private readonly IDashboardService _dashboardService; private readonly IWebHostEnvironment _env;
        public AdminController(IProductService p, ICategoryService c, IDashboardService d, IWebHostEnvironment env){_productService=p;_categoryService=c;_dashboardService=d;_env=env;}
        [HttpGet("")] public async Task<IActionResult> Index(){ViewData["Title"]="Administração";try{ViewData["Dashboard"]=await _dashboardService.GetSummaryAsync();}catch(HttpRequestException){}try{return View((await _productService.GetAllAsync()).OrderByDescending(p=>p.CreatedAt).ToList());}catch(HttpRequestException){ViewData["Message"]="Não foi possível carregar os produtos.";return View(new List<ProductDto>());}}
        [HttpGet("NewProduct")] public async Task<IActionResult> NewProduct(){ViewData["Title"]="Novo produto";return View(new ProductFormViewModel{Categories=await _categoryService.GetAllAsync(),AvailableSizes=new List<string>{"P","M","G","GG"}});}
        [HttpPost("NewProduct"),ValidateAntiForgeryToken] public async Task<IActionResult> NewProduct(ProductFormViewModel vm,Gender gender,List<IFormFile>? images){vm.Categories=await _categoryService.GetAllAsync();var urls=new List<string>();foreach(var image in images??new()){var u=await SaveProductImageAsync(image);if(u is null){ModelState.AddModelError(string.Empty,"Cada imagem deve ser JPG, JPEG, PNG ou WEBP e ter até 5 MB.");return View(vm);}urls.Add(u);}if(!ModelState.IsValid){vm.ImageUrls=vm.ImageUrls.Any()?vm.ImageUrls:urls;vm.KeepImageUrls=vm.KeepImageUrls.Any()?vm.KeepImageUrls:urls;return View(vm);}var categoryName=vm.Categories.FirstOrDefault(c=>c.Id==vm.CategoryId)?.Name;vm.AvailableSizes=ProductCatalogRules.NormalizeSizes(categoryName,vm.AvailableSizes);if(ProductCatalogRules.RequiresSize(categoryName)&&!vm.AvailableSizes.Any())ModelState.AddModelError(nameof(vm.AvailableSizes),"Selecione pelo menos um tamanho para esta categoria.");if(!ModelState.IsValid)return View(vm);var dto=new CreateProductDto{Name=vm.Name.Trim(),Description=vm.Description,Price=vm.Price,ImageUrl=urls.FirstOrDefault()??vm.ImageUrl,ImageUrls=urls,AvailableSizes=ProductCatalogRules.NormalizeSizes(vm.Categories.FirstOrDefault(c => c.Id == vm.CategoryId)?.Name, vm.AvailableSizes),Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured};await _productService.CreateAsync(dto);TempData["Success"]="Produto cadastrado com sucesso!";return RedirectToAction(nameof(Index));}
        [HttpGet("EditProduct/{id:int}")] public async Task<IActionResult> EditProduct(int id){var p=await _productService.GetByIdAsync(id);if(p is null)return NotFound();ViewData["Title"]="Editar produto";return View(new ProductFormViewModel{Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,KeepImageUrls=p.ImageUrls.ToList(),AvailableSizes=p.AvailableSizes.ToList(),CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=p.IsActive,Categories=await _categoryService.GetAllAsync(),Gender=p.Gender});}
        [HttpPost("EditProduct/{id:int}"),ValidateAntiForgeryToken] public async Task<IActionResult> EditProduct(int id,ProductFormViewModel vm,Gender gender,List<IFormFile>? images,List<string>? keepImageUrls){vm.Categories=await _categoryService.GetAllAsync();var current=await _productService.GetByIdAsync(id);if(current is null)return NotFound();var urls=(keepImageUrls??new List<string>()).Where(x=>current.ImageUrls.Contains(x)).Distinct().ToList();
            var removedImages = current.ImageUrls.Except(urls,StringComparer.OrdinalIgnoreCase).ToList();
            foreach(var image in images??new()){var u=await SaveProductImageAsync(image);if(u is null){ModelState.AddModelError(string.Empty,"Cada imagem deve ser JPG, JPEG, PNG ou WEBP e ter até 5 MB.");return View(vm);}urls.Add(u);}if(!ModelState.IsValid){vm.ImageUrls=vm.ImageUrls.Any()?vm.ImageUrls:urls;vm.KeepImageUrls=vm.KeepImageUrls.Any()?vm.KeepImageUrls:urls;return View(vm);}var categoryName=vm.Categories.FirstOrDefault(c=>c.Id==vm.CategoryId)?.Name;vm.AvailableSizes=ProductCatalogRules.NormalizeSizes(categoryName,vm.AvailableSizes);if(ProductCatalogRules.RequiresSize(categoryName)&&!vm.AvailableSizes.Any())ModelState.AddModelError(nameof(vm.AvailableSizes),"Selecione pelo menos um tamanho para esta categoria.");if(!ModelState.IsValid)return View(vm);var dto=new UpdateProductDto{Name=vm.Name.Trim(),Description=vm.Description,Price=vm.Price,ImageUrl=urls.FirstOrDefault(),ImageUrls=urls,AvailableSizes=ProductCatalogRules.NormalizeSizes(vm.Categories.FirstOrDefault(c => c.Id == vm.CategoryId)?.Name, vm.AvailableSizes),Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured,IsActive=vm.IsActive};await _productService.UpdateAsync(id,dto);
            foreach(var removed in removedImages) DeleteProductImage(removed);
            TempData["Success"]="Produto atualizado com sucesso!";return RedirectToAction(nameof(Index));}
        [HttpGet("DeleteProduct/{id:int}")] public async Task<IActionResult> DeleteProduct(int id){var p=await _productService.GetByIdAsync(id);return p is null?NotFound():View(p);}
        [HttpPost("DeleteProduct/{id:int}"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteProductConfirmed(int id){var current=await _productService.GetByIdAsync(id);if(current is null)return NotFound();var deleted=await _productService.DeleteAsync(id);if(deleted)foreach(var image in current.ImageUrls)DeleteProductImage(image);TempData["Success"]="Produto excluído com sucesso!";return RedirectToAction(nameof(Index));}
        private void DeleteProductImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/images/products/", StringComparison.OrdinalIgnoreCase)) return;
            var file = Path.Combine(_env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        }

        private async Task<string?> SaveProductImageAsync(IFormFile image){const long max=5*1024*1024;if(image.Length<=0||image.Length>max)return null;var ext=Path.GetExtension(image.FileName).ToLowerInvariant();var allowed=new Dictionary<string,string[]>{{".jpg",new[]{"image/jpeg"}},{".jpeg",new[]{"image/jpeg"}},{".png",new[]{"image/png"}},{".webp",new[]{"image/webp"}}};if(!allowed.TryGetValue(ext,out var types)||!types.Contains(image.ContentType,StringComparer.OrdinalIgnoreCase))return null;var folder=Path.Combine(_env.WebRootPath,"images","products");Directory.CreateDirectory(folder);var name=$"{Guid.NewGuid():N}{ext}";await using var stream=System.IO.File.Create(Path.Combine(folder,name));await image.CopyToAsync(stream);return $"/images/products/{name}";}
    }
}
