// =============================================================================
//  BetaFit.Desktop - Helpers/UiEndpointResolver.cs
// =============================================================================
//  CONCEITO: Descoberta Automática da URL do BetaFit.UI
//
// As imagens de produto (ImageUrl / ImageUrls) são salvas no banco como
// caminhos RELATIVOS (ex: "/images/products/boné-x.jpg"). Esses arquivos
// são servidos como conteúdo estático pelo projeto BetaFit.UI (porta 5085
// em desenvolvimento) — NÃO pela BetaFit.API (porta 5168).
//
// O Desktop já sabia resolver a URL da API via ApiEndpointResolver, mas não
// tinha nenhuma forma de descobrir a URL do UI. Esta classe replica a mesma
// estratégia de descoberta, só que apontando para o launchSettings.json do
// projeto BetaFit.UI.
//
// ====================================================
// PRIORIDADE DE RESOLUÇÃO
// ====================================================
//
//   PRIORIDADE 1 — launchSettings.json do projeto BetaFit.UI
//   PRIORIDADE 2 — appsettings.json do Desktop ("UiSettings.BaseUrl")
//   PRIORIDADE 3 — Null (URL não localizada; o Desktop cai no placeholder)
// =============================================================================

using System.Text.Json;

namespace BetaFit.Desktop.Helpers
{
    /// <summary>
    /// Responsável por descobrir automaticamente a URL base do BetaFit.UI
    /// (onde as imagens de produto ficam hospedadas como conteúdo estático).
    /// </summary>
    public static class UiEndpointResolver
    {
        // =====================================================================
        // CACHE
        // =====================================================================
        private static string? _resolvedUrl;
        private static bool _resolved = false;

        // =====================================================================
        // CONSTANTES
        // =====================================================================

        /// <summary>Nome do projeto do UI (usado para localizar o launchSettings.json).</summary>
        private const string UiProjectName = "BetaFit.UI";

        /// <summary>Caminho relativo do launchSettings dentro do projeto UI.</summary>
        private const string LaunchSettingsRelativePath =
            $"{UiProjectName}/Properties/launchSettings.json";

        /// <summary>Perfis preferidos do launchSettings (em ordem de preferência).</summary>
        private static readonly string[] PreferredProfiles = ["http", "https", "IIS Express"];

        // =====================================================================
        // MÉTODO PRINCIPAL
        // =====================================================================

        /// <summary>
        /// Resolve a URL base do BetaFit.UI seguindo a ordem de prioridade.
        /// O resultado é armazenado em cache após a primeira chamada.
        /// </summary>
        /// <returns>URL base do UI (ex: "http://localhost:5085") ou null se não encontrada.</returns>
        public static string? Resolve()
        {
            if (_resolved) return _resolvedUrl;

            _resolved = true;

            // ── PRIORIDADE 1: launchSettings.json ─────────────────────────────
            var fromLaunchSettings = TryResolveFromLaunchSettings();
            if (fromLaunchSettings != null)
            {
                _resolvedUrl = fromLaunchSettings;
                Log($"✅ UI localizado em: {_resolvedUrl}");
                Log($"   Origem: launchSettings.json do {UiProjectName}");
                return _resolvedUrl;
            }

            // ── PRIORIDADE 2: appsettings.json ────────────────────────────────
            var fromAppSettings = TryResolveFromAppSettings();
            if (fromAppSettings != null)
            {
                _resolvedUrl = fromAppSettings;
                Log($"✅ UI localizado em: {_resolvedUrl}");
                Log($"   Origem: appsettings.json (configuração manual)");
                return _resolvedUrl;
            }

            // ── PRIORIDADE 3: não encontrado ──────────────────────────────────
            Log("❌ URL do BetaFit.UI não foi localizada.");
            Log("   Verifique se BetaFit.UI/Properties/launchSettings.json existe");
            Log("   ou configure manualmente em appsettings.json → UiSettings.BaseUrl");
            Log("   Sem essa URL, as imagens de produto usarão o placeholder.");
            _resolvedUrl = null;
            return null;
        }

        /// <summary>
        /// Força a re-resolução na próxima chamada de <see cref="Resolve"/>.
        /// </summary>
        public static void Reset()
        {
            _resolved = false;
            _resolvedUrl = null;
        }

        // =====================================================================
        // PRIORIDADE 1 — launchSettings.json
        // =====================================================================

        private static string? TryResolveFromLaunchSettings()
        {
            foreach (var candidate in BuildLaunchSettingsCandidatePaths())
            {
                Log($"   🔍 Testando: {candidate}");

                if (!File.Exists(candidate)) continue;

                Log($"   📄 launchSettings.json encontrado em: {candidate}");

                var url = ParseLaunchSettings(candidate);
                if (url != null) return url;
            }

            return null;
        }

        private static List<string> BuildLaunchSettingsCandidatePaths()
        {
            var paths = new List<string>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // bin\Debug\net8.0-windows\ → subir níveis → raiz da solução
            var relativeLevels = new[] { 4, 5, 3, 6 };

            foreach (var levels in relativeLevels)
            {
                var dir = GoUpDirectories(baseDir, levels);
                if (dir != null)
                {
                    paths.Add(Path.Combine(dir, LaunchSettingsRelativePath));
                }
            }

            var solutionDir = Environment.GetEnvironmentVariable("SolutionDir");
            if (!string.IsNullOrEmpty(solutionDir))
            {
                paths.Add(Path.Combine(solutionDir, LaunchSettingsRelativePath));
            }

            paths.Add(Path.Combine(Directory.GetCurrentDirectory(), LaunchSettingsRelativePath));

            return paths;
        }

        private static string? ParseLaunchSettings(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("profiles", out var profiles))
                {
                    Log("   ⚠ launchSettings.json não contém seção 'profiles'");
                    return null;
                }

                foreach (var profileName in PreferredProfiles)
                {
                    if (!profiles.TryGetProperty(profileName, out var profile))
                        continue;

                    if (!profile.TryGetProperty("applicationUrl", out var urlProp))
                        continue;

                    var applicationUrl = urlProp.GetString();
                    if (string.IsNullOrWhiteSpace(applicationUrl))
                        continue;

                    var url = ExtractBestUrl(applicationUrl, profileName);
                    if (url != null)
                    {
                        Log($"   ✓ Perfil '{profileName}' → applicationUrl: {applicationUrl}");
                        Log($"   ✓ URL selecionada: {url}");
                        return url;
                    }
                }

                Log("   ⚠ Nenhum perfil com applicationUrl válida encontrado");
                return null;
            }
            catch (JsonException ex)
            {
                Log($"   ⚠ Erro ao parsear launchSettings.json: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Log($"   ⚠ Erro ao ler launchSettings.json: {ex.Message}");
                return null;
            }
        }

        private static string? ExtractBestUrl(string applicationUrl, string profileName)
        {
            var urls = applicationUrl
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .Where(u => !string.IsNullOrEmpty(u))
                .ToList();

            if (urls.Count == 0) return null;

            if (profileName == "http")
            {
                var httpUrl = urls.FirstOrDefault(u =>
                    u.StartsWith("http://", StringComparison.OrdinalIgnoreCase));
                return httpUrl ?? urls[0];
            }

            var httpsUrl = urls.FirstOrDefault(u =>
                u.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
            return httpsUrl ?? urls[0];
        }

        // =====================================================================
        // PRIORIDADE 2 — appsettings.json
        // =====================================================================

        /// <summary>
        /// Lê a URL do appsettings.json do Desktop (fallback manual).
        /// Espera o formato: { "UiSettings": { "BaseUrl": "http://..." } }
        /// </summary>
        private static string? TryResolveFromAppSettings()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (!File.Exists(path))
                {
                    Log("   ⚠ appsettings.json não encontrado");
                    return null;
                }

                var json = File.ReadAllText(path);
                json = RemoveJsonComments(json);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("UiSettings", out var uiSettings))
                {
                    if (uiSettings.TryGetProperty("BaseUrl", out var baseUrl))
                    {
                        var url = baseUrl.GetString();
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            Log($"   ✓ appsettings.json → UiSettings.BaseUrl: {url}");
                            return url;
                        }
                    }
                }

                Log("   ⚠ appsettings.json não contém UiSettings.BaseUrl");
                return null;
            }
            catch (Exception ex)
            {
                Log($"   ⚠ Erro ao ler appsettings.json: {ex.Message}");
                return null;
            }
        }

        // =====================================================================
        // UTILITÁRIOS
        // =====================================================================

        private static string? GoUpDirectories(string path, int levels)
        {
            var dir = new DirectoryInfo(path);
            for (int i = 0; i < levels; i++)
            {
                dir = dir.Parent;
                if (dir == null) return null;
            }
            return dir.FullName;
        }

        private static string RemoveJsonComments(string json)
        {
            var lines = json.Split('\n');
            var result = new System.Text.StringBuilder();
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                if (trimmed.StartsWith("//")) continue;
                var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
                result.AppendLine(commentIndex > 0 ? line[..commentIndex] : line);
            }
            return result.ToString();
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[UiEndpointResolver] {message}");
            Console.WriteLine($"[UiEndpointResolver] {message}");
        }
    }
}