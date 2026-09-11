using System.Net.Http.Headers;
using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Domain.Enums;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly BetaFitDbContext _db; private readonly IHttpClientFactory _factory; private readonly IConfiguration _config;
    public PaymentsController(BetaFitDbContext db,IHttpClientFactory factory,IConfiguration config){_db=db;_factory=factory;_config=config;}

    [Authorize]
    [HttpPost("preferences")]
    public async Task<ActionResult<PaymentPreferenceDto>> CreatePreference([FromBody] int orderId)
    {
        var userId=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var order=await _db.Orders.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id==orderId);
        if(order is null)return NotFound();
        if(order.UserId!=userId&&!User.IsInRole("Admin")&&!(User.IsInRole("Funcionario") || User.IsInRole("Estoquista")))return Forbid();
        if(order.PaymentStatus=="Paid")return BadRequest(new{message="Este pedido já foi pago."});
        var token=_config["MercadoPago:AccessToken"];
        if(string.IsNullOrWhiteSpace(token))return StatusCode(503,new{message="Gateway de pagamento não configurado. Defina MercadoPago:AccessToken."});
        var client=_factory.CreateClient("MercadoPago"); client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);
        var uiBase=_config["App:PublicBaseUrl"]?.TrimEnd('/')??"https://localhost:7000"; var apiBase=_config["Api:PublicBaseUrl"]?.TrimEnd('/')??Request.Scheme+"://"+Request.Host;
        var payload=new{items=order.Items.Select(i=>new{name=i.ProductName,quantity=i.Quantity,unit_price=i.UnitPrice,currency_id="BRL"}),external_reference=order.Id.ToString(),back_urls=new{success=$"{uiBase}/Orders/Details/{order.Id}",failure=$"{uiBase}/Orders/Details/{order.Id}",pending=$"{uiBase}/Orders/Details/{order.Id}"},auto_return="approved",notification_url=$"{apiBase}/api/payments/webhook"};
        var response=await client.PostAsJsonAsync("/checkout/preferences",payload); if(!response.IsSuccessStatusCode){var body=await response.Content.ReadAsStringAsync();return StatusCode(502,new{message="O gateway recusou a criação do checkout.",details=body});}
        var mp=await response.Content.ReadFromJsonAsync<MpPreference>(); if(mp is null||string.IsNullOrWhiteSpace(mp.id))return StatusCode(502,new{message="Resposta inválida do gateway."});
        order.PaymentId=mp.id; order.PaymentStatus="Pending"; await _db.SaveChangesAsync();
        return Ok(new PaymentPreferenceDto{OrderId=order.Id,PreferenceId=mp.id,CheckoutUrl=mp.init_point??mp.sandbox_init_point??string.Empty});
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] MpWebhook? notification)
    {
        if(notification?.data?.id is null)return Ok();
        if(!string.Equals(notification.type,"payment",StringComparison.OrdinalIgnoreCase))return Ok();
        var token=_config["MercadoPago:AccessToken"]; if(string.IsNullOrWhiteSpace(token))return Ok();
        var client=_factory.CreateClient("MercadoPago");client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);
        var response=await client.GetAsync($"/v1/payments/{notification.data.id}"); if(!response.IsSuccessStatusCode)return Ok();
        var payment=await response.Content.ReadFromJsonAsync<MpPayment>(); if(payment?.external_reference is null)return Ok();
        if(!int.TryParse(payment.external_reference,out var orderId))return Ok();
        var order=await _db.Orders.FirstOrDefaultAsync(x=>x.Id==orderId);if(order is null)return Ok();
        order.PaymentId=notification.data.id;order.PaymentStatus=payment.status??"Unknown";if(string.Equals(payment.status,"approved",StringComparison.OrdinalIgnoreCase))order.Status=OrderStatus.EmPreparacao;await _db.SaveChangesAsync();return Ok();
    }
    private sealed class MpPreference{public string? id{get;set;}public string? init_point{get;set;}public string? sandbox_init_point{get;set;}}
    public sealed class MpWebhook{public string? type{get;set;}public MpData? data{get;set;}}
    public sealed class MpData{public string? id{get;set;}}
    private sealed class MpPayment{public string? external_reference{get;set;}public string? status{get;set;}}
}
