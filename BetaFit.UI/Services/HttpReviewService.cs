using System.Net.Http.Json;
using System.Text.Json;
using BetaFit.Application.DTOs;
namespace BetaFit.UI.Services;
public class HttpReviewService
{
 private readonly HttpClient _http;
 private readonly ILogger<HttpReviewService> _logger;
 public string? LoadError { get; private set; }
 public HttpReviewService(HttpClient http,ILogger<HttpReviewService> logger){_http=http;_logger=logger;}
 private async Task<List<ReviewDto>> Read(string url){
  LoadError=null;
  try {using var response=await _http.GetAsync(url);response.EnsureSuccessStatusCode();return await response.Content.ReadFromJsonAsync<List<ReviewDto>>()??new();}
  catch(Exception ex) when(ex is HttpRequestException or JsonException or TaskCanceledException){_logger.LogError(ex,"Falha na consulta de avaliações {Url}",url);LoadError=ApiFailure.Message(ex,"as avaliações");return new();}
 }
 public Task<List<ReviewDto>> GetByOrderAsync(int id)=>Read($"api/reviews/order/{id}");
 public Task<List<ReviewDto>> GetByProductAsync(int id)=>Read($"api/reviews/product/{id}");
 public Task<List<ReviewDto>> ModerationAsync()=>Read("api/reviews/moderation");
 public async Task<(bool Ok,string Message)> CreateAsync(int orderId,int productId,CreateReviewDto dto){
  try{using var response=await _http.PostAsJsonAsync($"api/reviews/order/{orderId}/product/{productId}",dto);if(response.IsSuccessStatusCode)return(true,"Avaliação enviada com sucesso.");
   if((int)response.StatusCode<500){try{using var json=JsonDocument.Parse(await response.Content.ReadAsStringAsync());if(json.RootElement.TryGetProperty("message",out var message))return(false,message.GetString()??"Confira os campos da avaliação.");if(json.RootElement.TryGetProperty("errors",out var errors))return(false,string.Join(" ",errors.EnumerateObject().SelectMany(e=>e.Value.EnumerateArray().Select(x=>x.GetString()))));}catch(JsonException){}}
   _logger.LogError("API de avaliações retornou {Status}",response.StatusCode);return(false,"Não foi possível confirmar o envio. Recarregue o pedido para conferir se a avaliação foi registrada antes de tentar novamente.");
  }catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException){_logger.LogError(ex,"Falha ao enviar avaliação");return(false,"Não foi possível confirmar o envio. Confira suas avaliações antes de tentar novamente.");}
 }
 public async Task<bool> ModerateAsync(int id,string status){try{return(await _http.PutAsJsonAsync($"api/reviews/{id}/moderation",status)).IsSuccessStatusCode;}catch(HttpRequestException ex){_logger.LogError(ex,"Falha na moderação");return false;}}
}
