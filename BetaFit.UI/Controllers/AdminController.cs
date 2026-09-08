using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Application.ViewModels;
using BetaFit.Domain.Enums;
using BetaFit.UI.Helpers;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize(Roles = "Admin,Funcionario"), Route("Admin")]
public class AdminController : Controller
{
    private readonly IProductService _products;
    private readonly ICategoryService _categories;
    private readonly IDashboardService _dashboard;
    private readonly HttpSiteSettingsService _site;
    private readonly IWebHostEnvironment _env;

    public AdminController(IProductService products, ICategoryService categories, IDashboardService dashboard, HttpSiteSettingsService site, IWebHostEnvironment env)
    { _products=products; _categories=categories; _dashboard=dashboard; _site=site; _env=env; }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, int? categoryId, string? status)
    {
        ViewData["Title"]="Administração"; ViewData["Search"]=search; ViewData["SelectedCategoryId"]=categoryId; ViewData["SelectedStatus"]=status;
        try { if (User.IsInRole("Admin")) ViewData["Dashboard"] = await _dashboard.GetSummaryAsync(); } catch { }
        try
        {
            var categories=(await _categories.GetAllAsync()).ToList(); ViewData["Categories"]=categories;
            var query=(await _products.GetAllAsync()).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search)) query=query.Where(p=>p.Name.Contains(search,StringComparison.OrdinalIgnoreCase)||p.Description.Contains(search,StringComparison.OrdinalIgnoreCase));
            if(categoryId.HasValue) query=query.Where(p=>p.CategoryId==categoryId.Value);
            query=status?.ToLowerInvariant() switch { "active"=>query.Where(p=>p.IsActive&&p.Stock>0),"inactive"=>query.Where(p=>!p.IsActive),"out"=>query.Where(p=>p.Stock<=0),_=>query };
            return View(query.OrderByDescending(p=>p.CreatedAt).ToList());
        }
        catch(HttpRequestException) { ViewData["Message"]="Não foi possível carregar os produtos."; return View(new List<ProductDto>()); }
    }

    [HttpPost("Products/Bulk"), ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkProducts(List<int>? selectedIds, string action)
    {
        if (selectedIds is null || selectedIds.Count==0) { TempData["Error"]="Selecione pelo menos um produto."; return RedirectToAction(nameof(Index)); }
        var count=0;
        foreach(var id in selectedIds.Distinct().Take(200))
        {
            var p=await _products.GetByIdAsync(id); if(p is null) continue;
            if(action=="delete" && User.IsInRole("Admin")) { if(await _products.DeleteAsync(id)) count++; continue; }
            var stock=action=="out"?0:p.Stock; var active=action switch { "activate"=>true,"deactivate"=>false,_=>p.IsActive };
            if(await _products.UpdateAsync(id,new UpdateProductDto{Name=p.Name,Description=p.Description,Price=p.Price,Stock=stock,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,AvailableSizes=p.AvailableSizes,AvailableColors=p.AvailableColors,ColorImageUrls=p.ColorImageUrls,Gender=p.Gender,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=active}) is not null) count++;
        }
        TempData["Success"]=$"Ação aplicada em {count} produto(s)."; return RedirectToAction(nameof(Index));
    }

    [HttpGet("NewProduct")] public async Task<IActionResult> NewProduct() => View(new ProductFormViewModel{Categories=await _categories.GetAllAsync()});
    [HttpGet("EditProduct/{id:int}")]
    public async Task<IActionResult> EditProduct(int id)
    {
        var p=await _products.GetByIdAsync(id); if(p is null)return NotFound();
        return View(new ProductFormViewModel{Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,Stock=p.Stock,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,KeepImageUrls=p.ImageUrls.ToList(),AvailableSizes=p.AvailableSizes.ToList(),AvailableColors=p.AvailableColors.ToList(),ColorImageUrls=p.ColorImageUrls,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=p.IsActive,Categories=await _categories.GetAllAsync(),Gender=p.Gender});
    }

    [HttpPost("NewProduct"),ValidateAntiForgeryToken]
    public async Task<IActionResult> NewProduct(ProductFormViewModel vm, Gender gender, List<IFormFile>? images)
    {
        vm.Categories=await _categories.GetAllAsync(); vm.AvailableColors=NormalizeColors(vm.AvailableColors);
        var urls=await SaveImagesAsync(images, vm); vm.ColorImageUrls=await ReadColorImagesAsync(new()); PrepareProductForm(vm);
        if(!ModelState.IsValid){vm.ImageUrls=urls;vm.KeepImageUrls=urls;return View(vm);}
        var dto=new CreateProductDto{Name=vm.Name.Trim(),Description=vm.Description.Trim(),Price=vm.Price,Stock=vm.Stock,ImageUrl=urls.FirstOrDefault(),ImageUrls=urls,AvailableSizes=vm.AvailableSizes,AvailableColors=vm.AvailableColors,ColorImageUrls=vm.ColorImageUrls,Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured};
        try{await _products.CreateAsync(dto);}catch(InvalidOperationException ex){ModelState.AddModelError(string.Empty,ex.Message);vm.ImageUrls=urls;return View(vm);}
        TempData["Success"]="Produto cadastrado e clientes notificados.";return RedirectToAction(nameof(Index));
    }

    [HttpPost("EditProduct/{id:int}"),ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, ProductFormViewModel vm, Gender gender, List<IFormFile>? images, List<string>? keepImageUrls)
    {
        vm.Categories=await _categories.GetAllAsync(); vm.AvailableColors=NormalizeColors(vm.AvailableColors);
        var current=await _products.GetByIdAsync(id); if(current is null)return NotFound();
        var urls=(keepImageUrls??new()).Where(x=>current.ImageUrls.Contains(x)).Distinct().ToList(); urls.AddRange(await SaveImagesAsync(images,vm));
        vm.ColorImageUrls=await ReadColorImagesAsync(current.ColorImageUrls); PrepareProductForm(vm);
        if(!ModelState.IsValid){vm.ImageUrls=urls;vm.KeepImageUrls=urls;return View(vm);}
        var dto=new UpdateProductDto{Name=vm.Name.Trim(),Description=vm.Description.Trim(),Price=vm.Price,Stock=vm.Stock,ImageUrl=urls.FirstOrDefault(),ImageUrls=urls,AvailableSizes=vm.AvailableSizes,AvailableColors=vm.AvailableColors,ColorImageUrls=vm.ColorImageUrls,Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured,IsActive=vm.IsActive};
        try{if(await _products.UpdateAsync(id,dto) is null)return NotFound();}catch(InvalidOperationException ex){ModelState.AddModelError(string.Empty,ex.Message);vm.ImageUrls=urls;return View(vm);}
        TempData["Success"]="Produto atualizado.";return RedirectToAction(nameof(Index));
    }

    [HttpPost("MarkOutOfStock/{id:int}"),ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkOutOfStock(int id)
    {
        var p=await _products.GetByIdAsync(id); if(p is null)return NotFound();
        await _products.UpdateAsync(id,new UpdateProductDto{Name=p.Name,Description=p.Description,Price=p.Price,Stock=0,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,AvailableSizes=p.AvailableSizes,AvailableColors=p.AvailableColors,ColorImageUrls=p.ColorImageUrls,Gender=p.Gender,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=p.IsActive});
        TempData["Success"]="Produto marcado como esgotado.";return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles="Admin"), HttpGet("DeleteProduct/{id:int}")] public async Task<IActionResult> DeleteProduct(int id){var p=await _products.GetByIdAsync(id);return p is null?NotFound():View(p);}
    [Authorize(Roles="Admin"), HttpPost("DeleteProduct/{id:int}"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteProductConfirmed(int id){if(!await _products.DeleteAsync(id))return NotFound();TempData["Success"]="Produto excluído.";return RedirectToAction(nameof(Index));}

    [HttpGet("Categories")] public async Task<IActionResult> Categories()=>View((await _categories.GetAllAsync()).OrderBy(x=>x.Name).ToList());
    [HttpPost("Categories/Create"),ValidateAntiForgeryToken] public async Task<IActionResult> CreateCategory(string name){if(string.IsNullOrWhiteSpace(name)||name.Trim().Length is <2 or >100)TempData["Error"]="Informe um nome entre 2 e 100 caracteres.";else{await _categories.CreateAsync(new CreateCategoryDto{Name=name.Trim(),IsActive=true});TempData["Success"]="Categoria criada.";}return RedirectToAction(nameof(Categories));}
    [HttpPost("Categories/{id:int}/Update"),ValidateAntiForgeryToken] public async Task<IActionResult> UpdateCategory(int id,string name,bool isActive){var updated=await _categories.UpdateAsync(id,new UpdateCategoryDto{Name=(name??"").Trim(),IsActive=isActive});TempData[updated is null?"Error":"Success"]=updated is null?"Categoria não encontrada.":"Categoria atualizada.";return RedirectToAction(nameof(Categories));}
    [Authorize(Roles="Admin"),HttpPost("Categories/{id:int}/Delete"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteCategory(int id){var deleted=await _categories.DeleteAsync(id);TempData[deleted?"Success":"Error"]=deleted?"Categoria excluída.":"Não é possível excluir uma categoria que possui produtos.";return RedirectToAction(nameof(Categories));}

    [Authorize(Roles="Admin"),HttpGet("Site")] public async Task<IActionResult> Site()=>View(await _site.GetAsync());
    [Authorize(Roles="Admin"),HttpPost("Site"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Site(SiteSettingsDto vm)
    {
        for(var i=0;i<vm.HeroSlides.Count;i++){var media=Request.Form.Files.GetFile($"heroMedia_{i}");if(media is null||media.Length==0)continue;var url=await SaveSiteMediaAsync(media);if(url is null)ModelState.AddModelError(string.Empty,"Use imagens JPG/PNG/WEBP ou vídeos MP4/WEBM de até 25 MB.");else{vm.HeroSlides[i].MediaUrl=url;vm.HeroSlides[i].MediaType=Path.GetExtension(url).Equals(".mp4",StringComparison.OrdinalIgnoreCase)||Path.GetExtension(url).Equals(".webm",StringComparison.OrdinalIgnoreCase)?"video":"image";}}
        if(!ModelState.IsValid)return View(vm); TempData[await _site.UpdateAsync(vm)?"Success":"Error"]="Personalização do site atualizada.";return RedirectToAction(nameof(Site));
    }

    private void PrepareProductForm(ProductFormViewModel vm)
    {
        var category = vm.Categories.FirstOrDefault(c => c.Id == vm.CategoryId)?.Name;

        vm.AvailableSizes = ProductCatalogRules.NormalizeSizes(category, vm.AvailableSizes);

        if (ProductCatalogRules.RequiresSize(category) && !vm.AvailableSizes.Any())
        {
            ModelState.AddModelError(nameof(vm.AvailableSizes), "Selecione ao menos um tamanho.");
        }

        // A validação de vm.AvailableColors foi removida para tornar o campo opcional.
    }
    private async Task<List<string>> SaveImagesAsync(IEnumerable<IFormFile>? images,ProductFormViewModel vm){var list=new List<string>();foreach(var file in images??Array.Empty<IFormFile>()){var url=await SaveProductImageAsync(file);if(url is null)ModelState.AddModelError(string.Empty,"Cada foto deve ser JPG, PNG ou WEBP válida e ter até 5 MB.");else list.Add(url);}return list;}
    private async Task<Dictionary<string,string>> ReadColorImagesAsync(Dictionary<string,string> existing){var result=existing.Where(x=>Request.Form[$"keepColor_{Slug(x.Key)}"]=="true").ToDictionary(x=>x.Key,x=>x.Value,StringComparer.OrdinalIgnoreCase);foreach(var raw in Request.Form["AvailableColors"].ToArray()){var color=raw??string.Empty;if(string.IsNullOrWhiteSpace(color))continue;var file=Request.Form.Files.GetFile($"colorImage_{Slug(color)}");if(file is null||file.Length==0)continue;var url=await SaveProductImageAsync(file);if(url is null)ModelState.AddModelError(string.Empty,$"A foto da cor {color} é inválida.");else result[color]=url;}return result.Where(x=>Request.Form["AvailableColors"].Any(c=>string.Equals(c,x.Key,StringComparison.OrdinalIgnoreCase))).ToDictionary(x=>x.Key,x=>x.Value,StringComparer.OrdinalIgnoreCase);}
    private static string Slug(string value)=>new string(value.ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD).Where(c=>System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)!=System.Globalization.UnicodeCategory.NonSpacingMark&&char.IsLetterOrDigit(c)).ToArray());
    private static List<string> NormalizeColors(IEnumerable<string>? values)=>(values??Array.Empty<string>()).Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Take(20).ToList();
    private async Task<string?> SaveProductImageAsync(IFormFile image)=>await SaveMediaAsync(image,"products",5*1024*1024,new[]{".jpg",".jpeg",".png",".webp"});
    private async Task<string?> SaveSiteMediaAsync(IFormFile media)=>await SaveMediaAsync(media,"site",25*1024*1024,new[]{".jpg",".jpeg",".png",".webp",".mp4",".webm"});
    private async Task<string?> SaveMediaAsync(IFormFile file,string folderName,long max,string[] allowed)
    {
        if(file.Length<=0||file.Length>max)return null;var ext=Path.GetExtension(file.FileName).ToLowerInvariant();if(!allowed.Contains(ext))return null;
        await using(var input=file.OpenReadStream()){var header=new byte[12];var read=await input.ReadAsync(header);var image=ext is ".jpg" or ".jpeg"?read>=3&&header[0]==0xff&&header[1]==0xd8&&header[2]==0xff:ext==".png"?read>=8&&header[0]==0x89&&header[1]==0x50&&header[2]==0x4e&&header[3]==0x47:ext==".webp"?read>=12&&header[0]=='R'&&header[1]=='I'&&header[2]=='F'&&header[3]=='F'&&header[8]=='W'&&header[9]=='E'&&header[10]=='B'&&header[11]=='P':true;var video=ext==".mp4"?read>=8&&header[4]=='f'&&header[5]=='t'&&header[6]=='y'&&header[7]=='p':ext==".webm"?read>=4&&header[0]==0x1a&&header[1]==0x45&&header[2]==0xdf&&header[3]==0xa3:true;if(!image||!video)return null;}
        var folder=Path.Combine(_env.WebRootPath,"media",folderName);Directory.CreateDirectory(folder);var name=$"{Guid.NewGuid():N}{ext}";await using var stream=System.IO.File.Create(Path.Combine(folder,name));await file.CopyToAsync(stream);return $"/media/{folderName}/{name}";
    }
}
