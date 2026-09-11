using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BetaFit.UI.Services;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly HttpClient _api;private readonly HttpCartService _cart;
        private readonly IOrderService _orderService;
        private readonly HttpReviewService _reviewService;
        private readonly IWebHostEnvironment _env;

        public OrdersController(IOrderService orderService, HttpReviewService reviewService, IWebHostEnvironment env,IHttpClientFactory factory,HttpCartService cart)
        { _api=factory.CreateClient("ApiClient");_cart=cart; _orderService=orderService; _reviewService=reviewService; _env=env; }

        // GET /Orders/MyOrders
        public async Task<IActionResult> MyOrders(string? status,string? search){var userId=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??"";var all=(await _orderService.GetByUserIdAsync(userId)).ToList();foreach(var order in all.Where(o=>o.Status=="Entregue")){var reviews=await _reviewService.GetByOrderAsync(order.Id);foreach(var item in order.Items)item.Reviewed=reviews.Any(r=>r.ProductId==item.ProductId);}ViewData["SelectedStatus"]=status??"all";ViewData["Search"]=search;ViewData["OrderCounts"]=BetaFit.UI.Helpers.OrderJourney.Tabs.ToDictionary(t=>t.Key,t=>t.Key=="all"?all.Count:all.Count(o=>BetaFit.UI.Helpers.OrderJourney.Group(o)==t.Key));return View(all.Where(o=>string.IsNullOrEmpty(status)||status=="all"||BetaFit.UI.Helpers.OrderJourney.Group(o)==status).Where(o=>string.IsNullOrWhiteSpace(search)||"Beta Fit".Contains(search,StringComparison.OrdinalIgnoreCase)||o.Id.ToString()==search||o.Items.Any(i=>i.ProductName.Contains(search,StringComparison.OrdinalIgnoreCase))).ToList());}

        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<IActionResult> Staff(string? status)
        {
            var orders=(await _orderService.GetAllAsync()).AsEnumerable();ViewData["SelectedStatus"]=status;if(!string.IsNullOrWhiteSpace(status))orders=orders.Where(x=>string.Equals(x.Status,status,StringComparison.OrdinalIgnoreCase));return View("Staff", orders.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!await _orderService.UpdateStatusAsync(id, status)) TempData["Error"] = "Não foi possível atualizar o status do pedido.";
            else TempData["Success"] = "Status do pedido atualizado.";
            return RedirectToAction(nameof(Staff));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkStatus(List<int>? selectedIds, string status)
        {
            if(selectedIds is null || selectedIds.Count==0){TempData["Error"]="Selecione ao menos um pedido.";return RedirectToAction(nameof(Staff));}
            var updated=0; foreach(var id in selectedIds.Distinct().Take(200)) if(await _orderService.UpdateStatusAsync(id,status))updated++;
            TempData["Success"]=$"{updated} pedido(s) atualizado(s).";return RedirectToAction(nameof(Staff));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderService.CancelAsync(id, string.Empty);
            TempData[result.Ok ? "Success" : "Error"] = result.Ok ? "Pedido cancelado com sucesso." : result.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDelivery(int id, UpdateOrderDeliveryDto dto)
        {
            if(!ModelState.IsValid){TempData["Error"]="Revise os campos do endereço.";return RedirectToAction(nameof(Details),new{id});}
            var userId=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??string.Empty;
            var result=await _orderService.UpdateDeliveryAsync(id,userId,dto);
            TempData[result.Ok?"Success":"Error"]=result.Ok?"Endereço do pedido atualizado.":result.Message;
            return RedirectToAction(nameof(Details),new{id});
        }

        // GET /Orders/Details/{id}
        public async Task<IActionResult> Details(int id,bool admin=false)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Verifica se o pedido pertence ao usuário atual (segurança)
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (order.UserId != userId && !(User?.IsInRole("Admin") ?? false) && !((User?.IsInRole("Funcionario") ?? false) || (User?.IsInRole("Estoquista") ?? false)))
                return Forbid();

            ViewData["AdminContext"]=admin&&(User.IsInRole("Admin")||User.IsInRole("Estoquista")||User.IsInRole("Funcionario"));
            ViewData["Messages"]=await _api.GetFromJsonAsync<List<OrderMessageDto>>($"api/orders/{id}/messages")??new();
            ViewData["Reviews"] = await _reviewService.GetByOrderAsync(id);
            return View(order);
        }

        [HttpPost("Review"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int orderId, int productId, int rating, string? comment, List<IFormFile>? photos)
        {
            var order=await _orderService.GetByIdAsync(orderId); if(order is null)return NotFound();
            var userId=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??string.Empty;
            if(order.UserId!=userId)return Forbid();
            if(!order.Items.Any(x=>x.ProductId==productId))return BadRequest("Produto não pertence ao pedido.");
            if(rating is <1 or >5||BetaFit.Application.Services.ReviewPolicy.Validate(comment) is string){TempData["Error"]=BetaFit.Application.Services.ReviewPolicy.Validate(comment)??"Selecione de 1 a 5 estrelas.";return RedirectToAction(nameof(Details),new{id=orderId});}
            if((photos?.Count??0)>5){TempData["Error"]="Envie até 5 fotos.";return RedirectToAction(nameof(Details),new{id=orderId});}
            var urls=new List<string>(); var folder=Path.Combine(_env.WebRootPath,"images","reviews"); Directory.CreateDirectory(folder);
            foreach(var photo in photos??new())
            {
                if(photo.Length<=0||photo.Length>5*1024*1024) { TempData["Error"]="Cada foto deve ter até 5 MB."; return RedirectToAction(nameof(Details),new{id=orderId}); }
                var ext=Path.GetExtension(photo.FileName).ToLowerInvariant(); var allowed=new[]{".jpg",".jpeg",".png",".webp"}; if(!allowed.Contains(ext) || !await IsKnownImageAsync(photo, ext)){TempData["Error"]="Envie uma imagem JPG, JPEG, PNG ou WEBP válida.";return RedirectToAction(nameof(Details),new{id=orderId});}
                var name=$"{Guid.NewGuid():N}{ext}"; var target=Path.GetFullPath(Path.Combine(folder,name)); var root=Path.GetFullPath(folder)+Path.DirectorySeparatorChar; if(!target.StartsWith(root,StringComparison.Ordinal)) return BadRequest(); await using var stream=System.IO.File.Create(target); await photo.CopyToAsync(stream); urls.Add($"/images/reviews/{name}");
            }
            var result=await _reviewService.CreateAsync(orderId,productId,new CreateReviewDto{Rating=rating,Comment=comment??string.Empty,PhotoUrls=urls});
            if(!result.Ok)
            {
                foreach(var url in urls) { var file=Path.Combine(_env.WebRootPath,url.TrimStart('/').Replace('/',Path.DirectorySeparatorChar)); if(System.IO.File.Exists(file))System.IO.File.Delete(file); }
            }
            TempData[result.Ok?"Success":"Error"]=result.Message; return RedirectToAction(nameof(Details),new{id=orderId});
        }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> BuyAgain(int id){var o=await _orderService.GetByIdAsync(id);if(o==null)return NotFound();if(o.UserId!=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)return Forbid();var added=0;var errors=new List<string>();foreach(var item in o.Items){var r=await _cart.AddAsync(item.ProductId,item.Quantity,item.Size,item.Color);if(r.Ok)added++;else errors.Add(item.ProductName+": "+r.Message);}TempData["Success"]=$"{added} item(ns) adicionado(s) ao carrinho com preços atuais.";if(errors.Count>0)TempData["Error"]=string.Join(" ",errors);return RedirectToAction("Index","Cart");}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Journey(int id,string action,string? comment,int rating=5,string? code=null,bool admin=false){HttpResponseMessage response;switch(action){case "received":case "refund":response=await _api.PostAsJsonAsync($"api/orders/{id}/{action}",new{});break;case "experience":case "messages":response=await _api.PostAsJsonAsync($"api/orders/{id}/{action}",new OrderExperienceDto{Rating=rating,Comment=comment??""});break;case "tracking":response=await _api.PostAsJsonAsync($"api/orders/{id}/tracking",new TrackingDto{Code=code??"",Description=comment??""});break;default:return BadRequest();}TempData[response.IsSuccessStatusCode?"Success":"Error"]=response.IsSuccessStatusCode?"Informação registrada com sucesso.":"Não foi possível concluir. Confira o status do pedido e os campos informados.";return RedirectToAction(nameof(Details),new{id,admin});}

    private static async Task<bool> IsKnownImageAsync(IFormFile file, string ext)
    {
        await using var stream = file.OpenReadStream(); byte[] header = new byte[12]; var read=await stream.ReadAsync(header);
        return ext is ".jpg" or ".jpeg" ? read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF
            : ext == ".png" ? read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            : read >= 12 && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F' && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';
    }
}
}
