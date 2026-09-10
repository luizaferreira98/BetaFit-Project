// =============================================================================
// BetaFit.UI - Helpers/AppConfig.cs
// =============================================================================

namespace BetaFit.UI.Helpers
{
    public static class AppConfig
    {
        public static string ApiBaseUrl => ApiEndpointResolver.Resolve() ?? "http://localhost:5168";
    }
}
