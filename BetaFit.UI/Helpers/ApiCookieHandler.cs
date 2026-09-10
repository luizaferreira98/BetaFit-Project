// =============================================================================
// BetaFit.UI - Helpers/ApiCookieHandler.cs
// =============================================================================
//  CONCEITO IMPORTANTE: Propagação do Cookie de Autenticação
// A BetaFit.API autentica via ASP.NET Core Identity + Cookie (sem JWT).
// Quando o usuário faz login pela BetaFit.UI, o cookie retornado pela API
// é guardado numa Claim ("ApiCookie") do usuário local do MVC.
// Este Handler intercepta toda requisição HTTP feita para a API e reenvia
// esse cookie, para que a API reconheça o usuário autenticado.
// =============================================================================

namespace BetaFit.UI.Helpers
{
    public class ApiCookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiCookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity is { IsAuthenticated: true })
            {
                // A Claim "ApiCookie" guarda o cookie devolvido pela API no momento do Login
                var cookieClaim = user.FindFirst("ApiCookie");
                if (cookieClaim != null && !string.IsNullOrEmpty(cookieClaim.Value))
                {
                    request.Headers.Add("Cookie", cookieClaim.Value);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
