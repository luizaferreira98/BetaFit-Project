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
        private readonly IOrderService _orderService;
        private readonly HttpReviewService _reviewService;
        private readonly IWebHostEnvironment _env;

        public OrdersController(IOrderService orderService, HttpReviewService reviewService, IWebHostEnvironment env)
        { _orderService=orderService; _reviewService=reviewService; _env=env; }

        // GET /Orders/MyOrders
        public async Task<IActionResult> MyOrders(string? status)
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            try { var orders = await _orderService.GetByUserIdAsync(userId);ViewData["SelectedStatus"]=status;if(!string.IsNullOrWhiteSpace(status))orders=orders.Where(x=>string.Equals(x.Status,status,StringComparison.OrdinalIgnoreCase));return View(orders); }
            catch (HttpRequestException) { TempData["Error"] = "Não foi possível carregar seus pedidos. Verifique se a API e o banco estão atualizados."; return View(Array.Empty<OrderDto>()); }
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Staff(string? status)
        {
            var orders=(await _orderService.GetAllAsync()).AsEnumerable();ViewData["SelectedStatus"]=status;if(!string.IsNullOrWhiteSpace(status))orders=orders.Where(x=>string.Equals(x.Status,status,StringComparison.OrdinalIgnoreCase));return View("Staff", orders.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!await _orderService.UpdateStatusAsync(id, status)) TempData["Error"] = "Não foi possível atualizar o status do pedido.";
            else TempData["Success"] = "Status do pedido atualizado.";
            return RedirectToAction(nameof(Staff));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
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
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Verifica se o pedido pertence ao usuário atual (segurança)
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (order.UserId != userId && !(User?.IsInRole("Admin") ?? false) && !(User?.IsInRole("Funcionario") ?? false))
                return Forbid();

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
            rating=Math.Clamp(rating,1,5);
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

    private static async Task<bool> IsKnownImageAsync(IFormFile file, string ext)
    {
        await using var stream = file.OpenReadStream(); byte[] header = new byte[12]; var read=await stream.ReadAsync(header);
        return ext is ".jpg" or ".jpeg" ? read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF
            : ext == ".png" ? read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            : read >= 12 && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F' && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';
    }
}
}
