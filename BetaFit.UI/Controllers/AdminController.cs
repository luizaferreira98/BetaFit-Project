using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Application.ViewModels;
using BetaFit.Domain.Enums;
using BetaFit.UI.Helpers;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize(Roles = "Admin,Funcionario,Estoquista"), Route("Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _api;
    private readonly HttpReviewService _reviews;
    private readonly IProductService _products;
    private readonly ICategoryService _categories;
    private readonly IDashboardService _dashboard;
    private readonly HttpSiteSettingsService _site;
    private readonly IWebHostEnvironment _env;

    public AdminController(IProductService products, ICategoryService categories, IDashboardService dashboard, HttpSiteSettingsService site, IWebHostEnvironment env, HttpReviewService reviews,IHttpClientFactory factory)
    { _api=factory.CreateClient("ApiClient"); _reviews=reviews; _products=products; _categories=categories; _dashboard=dashboard; _site=site; _env=env; }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, int? categoryId, string? status,int page=1,int pageSize=20)
    {
        ViewData["Title"]="Administração"; ViewData["Search"]=search; ViewData["SelectedCategoryId"]=categoryId; ViewData["SelectedStatus"]=status;
        try { if (User.IsInRole("Admin")) ViewData["Dashboard"] = await _dashboard.GetSummaryAsync(); } catch { }
        try
        {
            var categories=(await _categories.GetAllAsync()).ToList(); ViewData["Categories"]=categories;
            var all=(await _products.GetAllAsync()).ToList();ViewData["TotalProducts"]=all.Count;ViewData["OutCount"]=all.Count(p=>p.Stock==0);ViewData["LowCount"]=all.Count(p=>p.Stock>0&&p.Stock<=p.LowStockThreshold);var query=all.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search)) query=query.Where(p=>p.Name.Contains(search,StringComparison.OrdinalIgnoreCase)||p.Description.Contains(search,StringComparison.OrdinalIgnoreCase));
            if(categoryId.HasValue) query=query.Where(p=>p.CategoryId==categoryId.Value);
            query=status?.ToLowerInvariant() switch { "active"=>query.Where(p=>p.IsActive&&p.Stock>0),"inactive"=>query.Where(p=>!p.IsActive),"out"=>query.Where(p=>p.Stock<=0),"low"=>query.Where(p=>p.Stock>0&&p.Stock<=p.LowStockThreshold),_=>query };
            var filtered=query.OrderByDescending(p=>p.CreatedAt).ToList();pageSize=new[]{10,20,50}.Contains(pageSize)?pageSize:20;var pages=Math.Max(1,(int)Math.Ceiling(filtered.Count/(double)pageSize));page=Math.Clamp(page,1,pages);ViewData["Filtered"]=filtered.Count;ViewData["Page"]=page;ViewData["Pages"]=pages;ViewData["PageSize"]=pageSize;return View(filtered.Skip((page-1)*pageSize).Take(pageSize).ToList());
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
            if(action=="delete") { if(await _products.DeleteAsync(id)) count++; continue; }
            var stock=action=="out"?0:p.Stock; var active=action switch { "activate"=>true,"deactivate"=>false,_=>p.IsActive };
            if(action=="out") foreach(var variant in p.Variants) variant.Stock=0;
            if(await _products.UpdateAsync(id,new UpdateProductDto{Name=p.Name,Description=p.Description,Price=p.Price,SalePrice=p.SalePrice,Sku=p.Sku,LowStockThreshold=p.LowStockThreshold,Variants=p.Variants,Stock=stock,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,AvailableSizes=p.AvailableSizes,AvailableColors=p.AvailableColors,ColorGalleries=p.ColorGalleries,SizeMeasurements=p.SizeMeasurements,MeasurementsAreDemo=p.MeasurementsAreDemo,ColorImageUrls=p.ColorImageUrls,Gender=p.Gender,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=active}) is not null) count++;
        }
        TempData["Success"]=$"Ação aplicada em {count} produto(s)."; return RedirectToAction(nameof(Index));
    }

    [HttpGet("NewProduct")] public async Task<IActionResult> NewProduct() => View(new ProductFormViewModel{IsActive=true,Categories=await _categories.GetAllAsync()});
    [HttpGet("EditProduct/{id:int}")]
    public async Task<IActionResult> EditProduct(int id)
    {
        var p=await _products.GetByIdAsync(id); if(p is null)return NotFound();
        return View(new ProductFormViewModel{Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,SalePrice=p.SalePrice,Sku=p.Sku,LowStockThreshold=p.LowStockThreshold,Variants=p.Variants,Stock=p.Stock,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,KeepImageUrls=p.ImageUrls.ToList(),AvailableSizes=p.AvailableSizes.ToList(),AvailableColors=p.AvailableColors.ToList(),ColorGalleries=p.ColorGalleries,SizeMeasurements=p.SizeMeasurements,MeasurementsAreDemo=p.MeasurementsAreDemo,ColorImageUrls=p.ColorImageUrls,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=p.IsActive,Categories=await _categories.GetAllAsync(),Gender=p.Gender});
    }

    [HttpPost("NewProduct"),ValidateAntiForgeryToken]
    public async Task<IActionResult> NewProduct(ProductFormViewModel vm, Gender gender, List<IFormFile>? images)
    {
        vm.Categories=await _categories.GetAllAsync(); vm.AvailableColors=NormalizeColors(vm.AvailableColors);
        var urls=await ReadMediaAsync(vm,null); PrepareProductForm(vm);
        if(!ModelState.IsValid){vm.ImageUrls=urls;vm.KeepImageUrls=urls;return View(vm);}
        var dto=new CreateProductDto{Name=vm.Name.Trim(),Description=vm.Description.Trim(),Price=vm.Price,SalePrice=vm.SalePrice,Sku=vm.Sku,LowStockThreshold=vm.LowStockThreshold,Variants=vm.Variants,Stock=vm.Stock,ImageUrl=urls.FirstOrDefault(),ImageUrls=urls,AvailableSizes=vm.AvailableSizes,AvailableColors=vm.AvailableColors,ColorGalleries=vm.ColorGalleries,SizeMeasurements=vm.SizeMeasurements,MeasurementsAreDemo=vm.MeasurementsAreDemo,ColorImageUrls=vm.ColorImageUrls,Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured,IsActive=vm.IsActive};
        try{await _products.CreateAsync(dto);}catch(InvalidOperationException ex){ModelState.AddModelError(string.Empty,ex.Message);vm.ImageUrls=urls;return View(vm);}
        TempData["Success"]="Produto cadastrado e clientes notificados.";return RedirectToAction(nameof(Index));
    }

    [HttpPost("EditProduct/{id:int}"),ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, ProductFormViewModel vm, Gender gender, List<IFormFile>? images, List<string>? keepImageUrls)
    {
        vm.Categories=await _categories.GetAllAsync(); vm.AvailableColors=NormalizeColors(vm.AvailableColors);
        var current=await _products.GetByIdAsync(id); if(current is null)return NotFound();
        var urls=await ReadMediaAsync(vm,current); PrepareProductForm(vm);
        if(!ModelState.IsValid){vm.ImageUrls=urls;vm.KeepImageUrls=urls;return View(vm);}
        var dto=new UpdateProductDto{Name=vm.Name.Trim(),Description=vm.Description.Trim(),Price=vm.Price,SalePrice=vm.SalePrice,Sku=vm.Sku,LowStockThreshold=vm.LowStockThreshold,Variants=vm.Variants,Stock=vm.Stock,ImageUrl=urls.FirstOrDefault(),ImageUrls=urls,AvailableSizes=vm.AvailableSizes,AvailableColors=vm.AvailableColors,ColorGalleries=vm.ColorGalleries,SizeMeasurements=vm.SizeMeasurements,MeasurementsAreDemo=vm.MeasurementsAreDemo,ColorImageUrls=vm.ColorImageUrls,Gender=gender,CategoryId=vm.CategoryId,IsFeatured=vm.IsFeatured,IsActive=vm.IsActive};
        try{if(await _products.UpdateAsync(id,dto) is null)return NotFound();}catch(InvalidOperationException ex){ModelState.AddModelError(string.Empty,ex.Message);vm.ImageUrls=urls;return View(vm);}
        TempData["Success"]="Produto atualizado.";return RedirectToAction(nameof(Index));
    }

    [HttpPost("MarkOutOfStock/{id:int}"),ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkOutOfStock(int id)
    {
        var p=await _products.GetByIdAsync(id); if(p is null)return NotFound();
        foreach(var variant in p.Variants) variant.Stock=0;
        await _products.UpdateAsync(id,new UpdateProductDto{Name=p.Name,Description=p.Description,Price=p.Price,SalePrice=p.SalePrice,Sku=p.Sku,LowStockThreshold=p.LowStockThreshold,Variants=p.Variants,Stock=0,ImageUrl=p.ImageUrl,ImageUrls=p.ImageUrls,AvailableSizes=p.AvailableSizes,AvailableColors=p.AvailableColors,ColorGalleries=p.ColorGalleries,SizeMeasurements=p.SizeMeasurements,MeasurementsAreDemo=p.MeasurementsAreDemo,ColorImageUrls=p.ColorImageUrls,Gender=p.Gender,CategoryId=p.CategoryId,IsFeatured=p.IsFeatured,IsActive=p.IsActive});
        TempData["Success"]="Produto marcado como esgotado.";return RedirectToAction(nameof(Index));
    }

    [HttpGet("DeleteProduct/{id:int}")] public async Task<IActionResult> DeleteProduct(int id){var p=await _products.GetByIdAsync(id);return p is null?NotFound():View(p);}
    [HttpPost("DeleteProduct/{id:int}"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteProductConfirmed(int id){if(!await _products.DeleteAsync(id))return NotFound();TempData["Success"]="Produto excluído.";return RedirectToAction(nameof(Index));}

    [HttpGet("Categories")] public async Task<IActionResult> Categories(string? search,string? status){var all=(await _categories.GetAllAsync()).ToList();ViewData["AllCategories"]=all;ViewData["Search"]=search;ViewData["SelectedStatus"]=status;return View(all.Where(c=>string.IsNullOrWhiteSpace(search)||c.Name.Contains(search,StringComparison.OrdinalIgnoreCase)).Where(c=>status switch{"active"=>c.IsActive,"inactive"=>!c.IsActive,"out"=>c.TotalStock==0,_=>true}).ToList());}
    [HttpPost("Categories/Create"),ValidateAntiForgeryToken] public async Task<IActionResult> CreateCategory(CreateCategoryDto dto){try{await _categories.CreateAsync(dto);TempData["Success"]="Categoria criada.";}catch(HttpRequestException){TempData["Error"]="Não foi possível criar: confira nome, slug único e categoria pai.";}return RedirectToAction(nameof(Categories));}
    [HttpPost("Categories/{id:int}/Update"),ValidateAntiForgeryToken] public async Task<IActionResult> UpdateCategory(int id,UpdateCategoryDto dto){var result=await _categories.UpdateAsync(id,dto);TempData[result==null?"Error":"Success"]=result==null?"Confira nome, slug único e hierarquia sem ciclos.":"Categoria atualizada.";return RedirectToAction(nameof(Categories));}
    [HttpPost("Categories/Bulk"),ValidateAntiForgeryToken] public async Task<IActionResult> BulkCategories(List<int> selectedIds,string action){int count=0;foreach(var id in selectedIds.Distinct().Take(200)){if(action=="delete"){if(await _categories.DeleteAsync(id))count++;}else if(action is "activate" or "deactivate"){if((await _api.PostAsJsonAsync($"api/management/categories/{id}/toggle",action=="activate")).IsSuccessStatusCode)count++;}}TempData["Success"]=$"{count} categoria(s) alterada(s). Categorias com produtos ou subcategorias não são excluídas.";return RedirectToAction(nameof(Categories));}
    [HttpPost("Categories/Reorder"),ValidateAntiForgeryToken] public async Task<IActionResult> ReorderCategories(List<int> ids)=>await Relay(await _api.PostAsJsonAsync("api/management/categories/reorder",ids));
    [HttpPost("Categories/{id:int}/Toggle"),ValidateAntiForgeryToken] public async Task<IActionResult> ToggleCategory(int id,bool active)=>await Relay(await _api.PostAsJsonAsync($"api/management/categories/{id}/toggle",active));
    [HttpPost("Products/{id:int}/Quick"),ValidateAntiForgeryToken] public async Task<IActionResult> QuickProduct(int id,string field,string value){object dto;if(field=="price"&&decimal.TryParse(value.Replace(',','.'),System.Globalization.NumberStyles.Number,System.Globalization.CultureInfo.InvariantCulture,out var price))dto=new{price};else if(field=="stock"&&int.TryParse(value,out var stock))dto=new{stock};else return BadRequest(new{message="Valor inválido."});return await Relay(await _api.PostAsJsonAsync($"api/management/products/{id}/quick",dto));}
    private async Task<IActionResult> Relay(HttpResponseMessage response){if(response.IsSuccessStatusCode)return Ok(new{ok=true});return StatusCode((int)response.StatusCode,new{message=await response.Content.ReadAsStringAsync()});}

    [Authorize(Roles="Admin"),HttpGet("Site")] public async Task<IActionResult> Site()=>View(await _site.GetAsync());
    [Authorize(Roles="Admin"),HttpPost("Site"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Site(SiteSettingsDto vm)
    {
        for(var i=0;i<vm.HeroSlides.Count;i++){var media=Request.Form.Files.GetFile($"heroMedia_{i}");if(media is null||media.Length==0)continue;var url=await SaveSiteMediaAsync(media);if(url is null)ModelState.AddModelError(string.Empty,"Use imagens JPG/PNG/WEBP ou vídeos MP4/WEBM de até 25 MB.");else{vm.HeroSlides[i].MediaUrl=url;vm.HeroSlides[i].MediaType=Path.GetExtension(url).Equals(".mp4",StringComparison.OrdinalIgnoreCase)||Path.GetExtension(url).Equals(".webm",StringComparison.OrdinalIgnoreCase)?"video":"image";}}
        if(!ModelState.IsValid)return View(vm); TempData[await _site.UpdateAsync(vm)?"Success":"Error"]="Personalização do site atualizada.";return RedirectToAction(nameof(Site));
    }

    [Authorize(Roles="Admin"),HttpGet("Reviews")]
    public async Task<IActionResult> Reviews() => View(await _reviews.ModerationAsync());
    [Authorize(Roles="Admin"),HttpPost("Reviews/{id:int}"),ValidateAntiForgeryToken]
    public async Task<IActionResult> ModerateReview(int id,string status)
    {var ok=await _reviews.ModerateAsync(id,status);TempData[ok?"Success":"Error"]=ok?"Visibilidade da avaliação atualizada.":"Não foi possível moderar a avaliação.";return RedirectToAction(nameof(Reviews));}

    private sealed class MediaRow {public string Url{get;set;}="";public string Color{get;set;}="";public string FileKey{get;set;}="";}
    private async Task<List<string>> ReadMediaAsync(ProductFormViewModel vm,ProductDto? current){
      List<MediaRow> rows;try{rows=System.Text.Json.JsonSerializer.Deserialize<List<MediaRow>>(Request.Form["mediaJson"].ToString(),new System.Text.Json.JsonSerializerOptions{PropertyNameCaseInsensitive=true})??new();}catch{ModelState.AddModelError("","Galeria inválida.");return new();}
      if(rows.Count>40){ModelState.AddModelError("","Use no máximo 40 fotos.");return new();}
      var allowed=current==null?new HashSet<string>():current.ImageUrls.Concat(current.ColorGalleries.Values.SelectMany(x=>x)).Concat(current.ColorImageUrls.Values).ToHashSet();
      var pending=System.Text.Json.JsonSerializer.Deserialize<List<string>>(HttpContext.Session.GetString("ProductDraftMedia")??"[]")??new();allowed.UnionWith(pending);
      vm.ColorGalleries=new(StringComparer.OrdinalIgnoreCase);vm.ColorImageUrls=new(StringComparer.OrdinalIgnoreCase);var urls=new List<string>();
      foreach(var row in rows){string? url=row.Url;if(!string.IsNullOrWhiteSpace(row.FileKey)){var file=Request.Form.Files.GetFile(row.FileKey);url=file==null?null:await SaveProductImageAsync(file);}else if(!allowed.Contains(url))url=null;
        if(url==null){ModelState.AddModelError("","Uma foto não é válida ou excede 5 MB.");continue;}urls.Add(url);if(!pending.Contains(url))pending.Add(url);HttpContext.Session.SetString("ProductDraftMedia",System.Text.Json.JsonSerializer.Serialize(pending.TakeLast(100)));
        if(!string.IsNullOrWhiteSpace(row.Color)){if(!vm.AvailableColors.Contains(row.Color)){ModelState.AddModelError("","Selecione a cor da foto nas cores disponíveis.");continue;}if(!vm.ColorGalleries.ContainsKey(row.Color))vm.ColorGalleries[row.Color]=new();vm.ColorGalleries[row.Color].Add(url);vm.ColorImageUrls.TryAdd(row.Color,url);}
      }
      vm.ImageUrls=urls.Distinct().ToList();return vm.ImageUrls;
    }
    private void PrepareProductForm(ProductFormViewModel vm)
    {
        try{vm.Variants=System.Text.Json.JsonSerializer.Deserialize<List<BetaFit.Domain.Entities.ProductVariant>>(Request.Form["variantsJson"].ToString(),new System.Text.Json.JsonSerializerOptions{PropertyNameCaseInsensitive=true})??new();}catch{ModelState.AddModelError("","Variações inválidas.");}
        var category = vm.Categories.FirstOrDefault(c => c.Id == vm.CategoryId)?.Name;

        vm.AvailableSizes = ProductCatalogRules.NormalizeSizes(category, vm.AvailableSizes);

        if (ProductCatalogRules.RequiresSize(category) && !vm.AvailableSizes.Any())
        {
            ModelState.AddModelError(nameof(vm.AvailableSizes), "Selecione ao menos um tamanho.");
        }

        var measurementJson=Request.Form["measurementsJson"].ToString();
        try {
            var parsed=System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,decimal>>>(measurementJson.Length==0?"{}":measurementJson)??new();
            if(parsed.Count>30 || parsed.Any(x=>x.Value.Count>12||x.Value.Any(v=>v.Value<=0||v.Value>500||v.Key.Length>80)))throw new System.Text.Json.JsonException();
            vm.SizeMeasurements=parsed.Where(x=>vm.AvailableSizes.Contains(x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
            if(!vm.MeasurementsAreDemo && vm.AvailableSizes.Any(x=>!vm.SizeMeasurements.TryGetValue(x,out var rows)||rows.Count==0))ModelState.AddModelError(string.Empty,"Preencha as medidas de todos os tamanhos antes de marcar como medidas reais.");
        } catch(System.Text.Json.JsonException){ModelState.AddModelError(string.Empty,"Tabela de medidas inválida. Use valores em cm maiores que zero, no formato indicado.");}
    }
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
