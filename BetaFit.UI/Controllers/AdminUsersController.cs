using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BetaFit.UI.Controllers;
[Authorize(Roles="Admin"),Route("Admin/Users")]
public class AdminUsersController : Controller
{
    private readonly HttpClient _api;
    public AdminUsersController(IHttpClientFactory factory)=>_api=factory.CreateClient("ApiClient");
    [HttpGet("")]
    public async Task<IActionResult> Index()=>View(await _api.GetFromJsonAsync<List<UsuarioDto>>("api/usuarios")??new());
    [HttpPost("Create"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUsuarioDto dto)
    {
        if(!ModelState.IsValid){TempData["Error"]="Revise e-mail, nome, senha e confirmação.";return RedirectToAction(nameof(Index));}
        return await Result(await _api.PostAsJsonAsync("api/usuarios",dto),"Usuário criado.");
    }
    [HttpPost("{id}/Update"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id,UpdateUsuarioDto dto)
    {
        if(id==User.FindFirstValue(ClaimTypes.NameIdentifier)){TempData["Error"]="Use Meu perfil para alterar sua própria conta.";return RedirectToAction(nameof(Index));}
        if(!ModelState.IsValid){TempData["Error"]="Revise os dados informados.";return RedirectToAction(nameof(Index));}
        return await Result(await _api.PutAsJsonAsync($"api/usuarios/{Uri.EscapeDataString(id)}",dto),"Usuário atualizado.");
    }
    [HttpPost("{id}/Delete"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if(id==User.FindFirstValue(ClaimTypes.NameIdentifier)){TempData["Error"]="Não é possível excluir sua própria conta por esta tela.";return RedirectToAction(nameof(Index));}
        return await Result(await _api.DeleteAsync($"api/usuarios/{Uri.EscapeDataString(id)}"),"Usuário excluído.");
    }
    private async Task<IActionResult> Result(HttpResponseMessage response,string success)
    {
        if(response.IsSuccessStatusCode)TempData["Success"]=success;
        else{var error=await response.Content.ReadFromJsonAsync<ApiError>();TempData["Error"]=error?.Message??"Não foi possível concluir a operação.";}
        return RedirectToAction(nameof(Index));
    }
    private sealed class ApiError{public string? Message{get;set;}}
}
