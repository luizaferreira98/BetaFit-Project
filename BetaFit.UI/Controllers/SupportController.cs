using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BetaFit.UI.Controllers;
[Authorize, Route("Support")]
public class SupportController(IHttpClientFactory factory) : Controller
{
    private HttpClient Api => factory.CreateClient("ApiClient");
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try{return View(await Api.GetFromJsonAsync<List<SupportThreadDto>>("api/support")??new());}
        catch(HttpRequestException){ViewData["Error"]="O atendimento está indisponível. Tente novamente em instantes.";return View(new List<SupportThreadDto>());}
    }
    [HttpPost("Create"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupportCreateDto dto)
    {
        if(!ModelState.IsValid){TempData["Error"]="Confira o assunto e a mensagem (até 1.000 caracteres).";return RedirectToAction(nameof(Index));}
        try{
            var response=await Api.PostAsJsonAsync("api/support",dto);
            if(response.IsSuccessStatusCode){var thread=await response.Content.ReadFromJsonAsync<SupportThreadDto>();return RedirectToAction(nameof(Details),new{id=thread!.Id});}
        }catch(HttpRequestException){}
        TempData["Error"]="Não foi possível enviar. Tente novamente.";return RedirectToAction(nameof(Index));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        try{var response=await Api.GetAsync($"api/support/{id}");if(!response.IsSuccessStatusCode)return NotFound();return View(await response.Content.ReadFromJsonAsync<SupportThreadDto>());}
        catch(HttpRequestException){TempData["Error"]="Conversa indisponível. Tente novamente.";return RedirectToAction(nameof(Index));}
    }
    [HttpGet("{id:int}/Messages"),ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
    public async Task<IActionResult> Messages(int id)
    {
        try{var response=await Api.GetAsync($"api/support/{id}");if(!response.IsSuccessStatusCode)return StatusCode((int)response.StatusCode);return Json(await response.Content.ReadFromJsonAsync<SupportThreadDto>());}
        catch(HttpRequestException){return StatusCode(503);}
    }
    [HttpPost("{id:int}/Send"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int id, SupportSendDto dto)
    {
        var ajax=Request.Headers.Accept.ToString().Contains("application/json");
        if(!ModelState.IsValid)return BadRequest(new{message="Escreva de 1 a 1.000 caracteres."});
        try{var response=await Api.PostAsJsonAsync($"api/support/{id}/messages",dto);if(response.IsSuccessStatusCode)return ajax?Ok():RedirectToAction(nameof(Details),new{id});}
        catch(HttpRequestException){}
        if(ajax)return StatusCode(503,new{message="Não foi possível enviar. Sua mensagem foi mantida para tentar novamente."});
        TempData["Error"]="Não foi possível enviar. Tente novamente.";return RedirectToAction(nameof(Details),new{id});
    }
}
