using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using System.Text;
using System.Globalization;
namespace BetaFit.Application.Services;
public class CategoryService : ICategoryService
{
 readonly ICategoryRepository repo; readonly IOrderRepository orders;
 public CategoryService(ICategoryRepository categoryRepository,IOrderRepository orderRepository){repo=categoryRepository;orders=orderRepository;}
 public async Task<IEnumerable<CategoryDto>> GetAllAsync(){
  var all=(await repo.GetAllAsync()).ToList();var month=new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
  var sales=(await orders.GetAllAsync()).Where(o=>o.CreatedAt>=month&&o.Status!=BetaFit.Domain.Enums.OrderStatus.Cancelado&&o.PaymentStatus.Contains("Pago",StringComparison.OrdinalIgnoreCase)).SelectMany(o=>o.Items).GroupBy(i=>i.ProductId).ToDictionary(g=>g.Key,g=>g.Sum(i=>i.Quantity));
  bool HasStock(Category c,HashSet<int> visited){if(!visited.Add(c.Id))return false;return c.Products.Any(p=>p.IsActive&&p.Stock>0)||all.Where(x=>x.ParentId==c.Id&&x.IsActive).Any(x=>HasStock(x,visited));}
  bool Visible(Category c,HashSet<int> path){if(!path.Add(c.Id)||!c.IsActive||c.HideWhenOutOfStock&&!HasStock(c,new()))return false;return c.ParentId is not int id||all.FirstOrDefault(p=>p.Id==id) is Category parent&&Visible(parent,path);}
  return all.OrderBy(c=>c.SortOrder).ThenBy(c=>c.Id).Select(c=>new CategoryDto{Id=c.Id,Name=c.Name,IsActive=c.IsActive,Slug=string.IsNullOrWhiteSpace(c.Slug)?Slugify(c.Name):c.Slug,ParentId=c.ParentId,ParentName=all.FirstOrDefault(p=>p.Id==c.ParentId)?.Name,SortOrder=c.SortOrder,HideWhenOutOfStock=c.HideWhenOutOfStock,IsVisible=Visible(c,new()),TotalStock=c.Products.Sum(p=>p.Stock),ProductCount=c.Products.Count,ActiveProducts=c.Products.Count(p=>p.IsActive&&p.Stock>0),InactiveProducts=c.Products.Count(p=>!p.IsActive),OutOfStockProducts=c.Products.Count(p=>p.Stock==0),MonthSales=c.Products.Sum(p=>sales.GetValueOrDefault(p.Id)),CreatedAt=c.CreatedAt}).ToList();
 }
 public async Task<CategoryDto?> GetByIdAsync(int id)=>(await GetAllAsync()).FirstOrDefault(c=>c.Id==id);
 public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto){var c=new Category();await Apply(c,dto.Name,dto.Slug,dto.ParentId,dto.SortOrder,dto.IsActive,dto.HideWhenOutOfStock);await repo.AddAsync(c);return (await GetByIdAsync(c.Id))!;}
 public async Task<CategoryDto?> UpdateAsync(int id,UpdateCategoryDto dto){var c=await repo.GetByIdAsync(id);if(c==null)return null;await Apply(c,dto.Name,dto.Slug,dto.ParentId,dto.SortOrder,dto.IsActive,dto.HideWhenOutOfStock);await repo.UpdateAsync(c);return await GetByIdAsync(id);}
 async Task Apply(Category c,string name,string slug,int? parent,int order,bool active,bool hide){
  var all=(await repo.GetAllAsync()).ToList();var normalized=Slugify(string.IsNullOrWhiteSpace(slug)?name:slug);
  if(name.Trim().Length is <2 or >100||normalized.Length is <2 or >100)throw new InvalidOperationException("Informe nome e slug entre 2 e 100 caracteres.");
  if(all.Any(x=>x.Id!=c.Id&&(x.Slug==normalized||string.IsNullOrEmpty(x.Slug)&&Slugify(x.Name)==normalized)))throw new InvalidOperationException("Esta URL já pertence a outra categoria.");
  var seen=new HashSet<int>{c.Id};var node=parent;while(node.HasValue){if(!seen.Add(node.Value))throw new InvalidOperationException("Uma categoria não pode ser filha de si mesma ou criar um ciclo.");var p=all.FirstOrDefault(x=>x.Id==node);if(p==null)throw new InvalidOperationException("Categoria pai não encontrada.");node=p.ParentId;}
  c.Name=name.Trim();c.Slug=normalized;c.ParentId=parent;c.SortOrder=order;c.IsActive=active;c.HideWhenOutOfStock=hide;
 }
 public async Task<bool> DeleteAsync(int id){var c=await repo.GetByIdAsync(id);if(c==null||c.Products.Any()||(await repo.GetAllAsync()).Any(x=>x.ParentId==id))return false;await repo.DeleteAsync(id);return true;}
 public Task<int> CountAsync()=>repo.CountAsync();
 public static string Slugify(string value){var s=new string(value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD).Where(c=>CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark).ToArray());return System.Text.RegularExpressions.Regex.Replace(s,"[^a-z0-9]+","-").Trim('-');}
}
