// =============================================================================
//  BetaFit.Desktop - Helpers/ApiEndpointResolver.cs
// =============================================================================
//  CONCEITO: Descoberta Automática da URL da API
//
// Esta classe resolve AUTOMATICAMENTE a URL correta da API BetaFit,
// eliminando a necessidade de portas hardcoded no código.
//
// ====================================================
// PRIORIDADE DE RESOLUÇÃO
// ====================================================
//
//   PRIORIDADE 1 — launchSettings.json do projeto BetaFit.API
//     Lê o arquivo Properties/launchSettings.json da API e extrai
//     a applicationUrl do perfil ativo.
//      Funciona automaticamente mesmo que o VS mude a porta.
//
//   PRIORIDADE 2 — appsettings.json do Desktop
//     Lê "ApiSettings.BaseUrl" do arquivo de configuração local.
//      Fallback manual configurável sem recompilar.
//
//   PRIORIDADE 3 — Null (URL não localizada)
//     Retorna null. Program.cs exibirá uma mensagem amigável.
//      Nunca lança exceção sem tratamento.
//
// ====================================================
// COMO FUNCIONA A LOCALIZAÇÃO DO launchSettings.json
// ====================================================
//
// Durante desenvolvimento, o executável está em:
//   BetaFit.Desktop/bin/Debug/net8.0-windows/
//
// Subindo 4 níveis chega à raiz da solução:
//   BetaFit/
//
// O resolver testa múltiplos caminhos candidatos até encontrar
// o arquivo launchSettings.json do projeto API.
//
// ====================================================
// ESCALABILIDADE (produção)
// ====================================================
//
// Para usar em produção, basta definir no appsettings.json:
//   "ApiSettings": { "BaseUrl": "https://api.betafit.com" }
//
// O resolver detecta automaticamente que o launchSettings não
// existe (ambiente de produção) e usa o appsettings.
// =============================================================================

using System.Text.Json;

namespace BetaFit.Desktop.Helpers
{
    /// <summary>
    /// Responsável por descobrir automaticamente a URL base da API BetaFit.
    /// Elimina a necessidade de portas hardcoded no código.
    /// </summary>
    /// <remarks>
    /// Ordem de resolução:
    ///   1. launchSettings.json do projeto BetaFit.API (desenvolvimento)
    ///   2. appsettings.json do Desktop (fallback configurável)
    ///   3. null (URL não encontrada — app exibe mensagem amigável)
    /// </remarks>
    public static class ApiEndpointResolver
    {
        // =====================================================================
        // CACHE
        // =====================================================================

        // Armazena o resultado após a primeira resolução (evita re-leitura)
        private static string? _resolvedUrl;
        private static bool _resolved = false;

        // =====================================================================
        // CONSTANTES
        // =====================================================================

        /// <summary>
        /// Nome do projeto da API (usado para localizar o launchSettings.json).
        /// </summary>
        private const string ApiProjectName = "BetaFit.API";

        /// <summary>
        /// Caminho relativo do launchSettings dentro do projeto da API.
        /// </summary>
        private const string LaunchSettingsRelativePath =
            $"{ApiProjectName}/Properties/launchSettings.json";

        /// <summary>
        /// Perfis preferidos do launchSettings (em ordem de preferência).
        /// "http" é preferido em desenvolvimento para evitar erros de SSL.
        /// </summary>
        private static readonly string[] PreferredProfiles = ["http", "https", "IIS Express"];

        // =====================================================================
        // MÉTODO PRINCIPAL
        // =====================================================================

        /// <summary>
        /// Resolve a URL base da API seguindo a ordem de prioridade.
        /// O resultado é armazenado em cache após a primeira chamada.
        /// </summary>
        /// <returns>
        /// URL base da API (ex: "http://localhost:5168") ou null se não encontrada.
        /// </returns>
        public static string? Resolve()
        {
            // Retorna cache se já foi resolvido
            if (_resolved) return _resolvedUrl;

            _resolved = true;

            // ── PRIORIDADE 1: launchSettings.json ─────────────────────────────
            var fromLaunchSettings = TryResolveFromLaunchSettings();
            if (fromLaunchSettings != null)
            {
                _resolvedUrl = fromLaunchSettings;
                Log($"✅ API localizada em: {_resolvedUrl}");
                Log($"   Origem: launchSettings.json do {ApiProjectName}");
                return _resolvedUrl;
            }

            // ── PRIORIDADE 2: appsettings.json ────────────────────────────────
            var fromAppSettings = TryResolveFromAppSettings();
            if (fromAppSettings != null)
            {
                _resolvedUrl = fromAppSettings;
                Log($"✅ API localizada em: {_resolvedUrl}");
                Log($"   Origem: appsettings.json (configuração manual)");
                return _resolvedUrl;
            }

            // ── PRIORIDADE 3: não encontrado ──────────────────────────────────
            Log("❌ URL da API não foi localizada.");
            Log("   Verifique se SenacGames.API/Properties/launchSettings.json existe");
            Log("   ou configure manualmente em appsettings.json → ApiSettings.BaseUrl");
            _resolvedUrl = null;
            return null;
        }

        /// <summary>
        /// Força a re-resolução na próxima chamada de <see cref="Resolve"/>.
        /// Útil em testes ou quando as configurações mudam em tempo de execução.
        /// </summary>
        public static void Reset()
        {
            _resolved = false;
            _resolvedUrl = null;
        }

        // =====================================================================
        // PRIORIDADE 1 — launchSettings.json
        // =====================================================================

        /// <summary>
        /// Tenta localizar e parsear o launchSettings.json do projeto BetaFit.API.
        /// Testa múltiplos caminhos candidatos até encontrar o arquivo.
        /// </summary>
        private static string? TryResolveFromLaunchSettings()
        {
            var candidates = BuildLaunchSettingsCandidatePaths();

            foreach (var candidate in candidates)
            {
                Log($"   🔍 Testando: {candidate}");

                if (!File.Exists(candidate)) continue;

                Log($"   📄 launchSettings.json encontrado em: {candidate}");

                var url = ParseLaunchSettings(candidate);
                if (url != null) return url;
            }

            return null;
        }

        /// <summary>
        /// Constrói a lista de caminhos candidatos para o launchSettings.json.
        /// Cobre os cenários mais comuns de execução (Debug, Release, testes).
        /// </summary>
        private static List<string> BuildLaunchSettingsCandidatePaths()
        {
            var paths = new List<string>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Caminhos baseados na posição do executável
            // bin\Debug\net8.0-windows\  subir 4 níveis  raiz da solução
            var relativeLevels = new[] { 4, 5, 3, 6 };

            foreach (var levels in relativeLevels)
            {
                var dir = GoUpDirectories(baseDir, levels);
                if (dir != null)
                {
                    paths.Add(Path.Combine(dir, LaunchSettingsRelativePath));
                }
            }

            // Caminho via variável de ambiente (quando executado via MSBuild/scripts)
            var solutionDir = Environment.GetEnvironmentVariable("SolutionDir");
            if (!string.IsNullOrEmpty(solutionDir))
            {
                paths.Add(Path.Combine(solutionDir, LaunchSettingsRelativePath));
            }

            // Caminho relativo ao diretório de trabalho atual
            paths.Add(Path.Combine(
                Directory.GetCurrentDirectory(),
                LaunchSettingsRelativePath));

            return paths;
        }

        /// <summary>
        /// Parseia o launchSettings.json e extrai a URL de acordo com a
        /// ordem de preferência de perfis.
        /// </summary>
        /// <param name="filePath">Caminho absoluto do launchSettings.json</param>
        /// <returns>URL base ou null se não encontrada no arquivo</returns>
        private static string? ParseLaunchSettings(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Navega até "profiles"
                if (!root.TryGetProperty("profiles", out var profiles))
                {
                    Log("   ⚠ launchSettings.json não contém seção 'profiles'");
                    return null;
                }

                // Tenta cada perfil em ordem de preferência
                foreach (var profileName in PreferredProfiles)
                {
                    if (!profiles.TryGetProperty(profileName, out var profile))
                        continue;

                    if (!profile.TryGetProperty("applicationUrl", out var urlProp))
                        continue;

                    var applicationUrl = urlProp.GetString();
                    if (string.IsNullOrWhiteSpace(applicationUrl))
                        continue;

                    // applicationUrl pode conter múltiplas URLs separadas por ";"
                    // Ex: "https://localhost:7033;http://localhost:5223"
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

        /// <summary>
        /// Extrai a URL mais adequada de uma string que pode conter múltiplas URLs.
        /// Para perfil "http": prefere http://
        /// Para perfil "https": prefere https://, mas aceita http:// como fallback
        /// </summary>
        private static string? ExtractBestUrl(string applicationUrl, string profileName)
        {
            // Separa múltiplas URLs (ex: "https://localhost:7033;http://localhost:5168")
            var urls = applicationUrl
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .Where(u => !string.IsNullOrEmpty(u))
                .ToList();

            if (urls.Count == 0) return null;

            // Para perfil "http": usa a primeira URL HTTP disponível
            if (profileName == "http")
            {
                var httpUrl = urls.FirstOrDefault(u =>
                    u.StartsWith("http://", StringComparison.OrdinalIgnoreCase));
                return httpUrl ?? urls[0];
            }

            // Para perfil "https": prefere HTTPS (sem problemas de certificado em dev
            // pois o handler já aceita qualquer certificado)
            var httpsUrl = urls.FirstOrDefault(u =>
                u.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
            return httpsUrl ?? urls[0];
        }

        // =====================================================================
        // PRIORIDADE 2 — appsettings.json
        // =====================================================================

        /// <summary>
        /// Lê a URL do appsettings.json do Desktop (fallback manual).
        /// Espera o formato: { "ApiSettings": { "BaseUrl": "http://..." } }
        /// </summary>
        private static string? TryResolveFromAppSettings()
        {
            try
            {
                var path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "appsettings.json");

                if (!File.Exists(path))
                {
                    Log("   ⚠ appsettings.json não encontrado");
                    return null;
                }

                var json = File.ReadAllText(path);
                // Remove comentários de linha (// ...) que não são JSON padrão
                json = RemoveJsonComments(json);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Formato novo: ApiSettings.BaseUrl
                if (root.TryGetProperty("ApiSettings", out var apiSettings))
                {
                    if (apiSettings.TryGetProperty("BaseUrl", out var baseUrl))
                    {
                        var url = baseUrl.GetString();
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            Log($"   ✓ appsettings.json → ApiSettings.BaseUrl: {url}");
                            return url;
                        }
                    }
                }

                // Formato legado: ApiBaseUrl (compatibilidade retroativa)
                if (root.TryGetProperty("ApiBaseUrl", out var legacyUrl))
                {
                    var url = legacyUrl.GetString();
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        Log($"   ✓ appsettings.json → ApiBaseUrl (legado): {url}");
                        return url;
                    }
                }

                Log("   ⚠ appsettings.json não contém ApiSettings.BaseUrl nem ApiBaseUrl");
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

        /// <summary>
        /// Sobe N níveis a partir de um diretório base.
        /// </summary>
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

        /// <summary>
        /// Remove comentários de linha // do JSON (não é JSON padrão, mas
        /// o Visual Studio gera appsettings.json com comentários).
        /// </summary>
        private static string RemoveJsonComments(string json)
        {
            var lines = json.Split('\n');
            var result = new System.Text.StringBuilder();
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                // Pula linhas que são apenas comentários
                if (trimmed.StartsWith("//")) continue;
                // Remove comentário inline após código válido
                var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
                if (commentIndex > 0)
                {
                    result.AppendLine(line[..commentIndex]);
                }
                else
                {
                    result.AppendLine(line);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Escreve mensagem de diagnóstico no Output do Visual Studio.
        /// Visível em: View  Output  Debug
        /// </summary>
        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[ApiEndpointResolver] {message}");
            // Também no console (útil quando rodado fora do VS)
            Console.WriteLine($"[ApiEndpointResolver] {message}");
        }
    }
}