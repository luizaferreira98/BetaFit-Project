// =============================================================================
// BetaFit.UI - Helpers/AppConfig.cs
// =============================================================================
//  CONCEITO: Resolução segura da URL da API para ambientes distintos

namespace BetaFit.UI.Helpers
{
    public static class AppConfig
    {
        /// <summary>
        /// Resolve a URL da API com fallback seguro para ambientes de produção.
        /// Em desenvolvimento: tenta launchSettings.json do projeto BetaFit.API
        /// Em produção: usa exclusivamente appsettings.json
        /// </summary>
        public static string ApiBaseUrl { get; set; } = string.Empty;

        public static void Initialize(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string? resolvedUrl = null;

            // Em produção, ignora launchSettings e usa apenas appsettings
            if (environment.IsDevelopment())
            {
                resolvedUrl = ApiEndpointResolver.Resolve();
            }

            // Fallback: sempre tenta appsettings.json
            if (string.IsNullOrEmpty(resolvedUrl))
            {
                resolvedUrl = configuration["ApiSettings:BaseUrl"];
            }

            // Validação final
            if (string.IsNullOrEmpty(resolvedUrl))
            {
                throw new InvalidOperationException(
                    "❌ Falha ao resolver URL da API. Verifique: " +
                    "1. appsettings.json contém 'ApiSettings:BaseUrl'? " +
                    "2. Em produção, a variável de ambiente está definida? " +
                    "3. Em desenvolvimento, launchSettings.json da API é acessível?"
                );
            }

            // Garante URL com trailing slash
            ApiBaseUrl = resolvedUrl.TrimEnd('/') + "/";
        }
    }
}