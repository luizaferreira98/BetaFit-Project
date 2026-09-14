using System.Net;

namespace BetaFit.UI.Services;

public static class ApiFailure
{
    public static string Message(Exception error, string resource) => error switch
    {
        HttpRequestException { StatusCode: HttpStatusCode.Unauthorized } => "Sua sessão expirou. Entre novamente na conta para continuar.",
        HttpRequestException { StatusCode: HttpStatusCode.Forbidden } => "Sua conta não tem permissão para acessar este recurso.",
        HttpRequestException { StatusCode: HttpStatusCode.NotFound } => "Este recurso não foi encontrado na API. Atualize e reinicie a API junto com a interface.",
        TaskCanceledException => $"O carregamento de {resource} demorou mais que o esperado. Tente novamente.",
        System.Text.Json.JsonException => $"A API retornou dados inválidos de {resource}. Verifique se a API e a interface estão na mesma versão.",
        _ => $"Não foi possível carregar {resource}. Tente novamente em instantes."
    };
}
