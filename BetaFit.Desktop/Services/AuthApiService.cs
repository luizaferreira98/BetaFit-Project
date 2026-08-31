// =============================================================================
// BetaFit.Desktop - Services/AuthApiService.cs
// =============================================================================
//  CONCEITO: Service de Autenticação
//
// Esta classe é responsável por se comunicar com os endpoints de autenticação da API:
//   POST /api/auth/login     Fazer login
//   POST /api/auth/logout    Fazer logout
//   GET  /api/auth/me        Buscar dados do usuário atual
//
// Por que criar um Service separado?
//    Separa a lógica de comunicação HTTP da lógica de interface
//    Facilita reutilização em múltiplos formulários
//    Facilita testes e manutenção
//    Segue o princípio de Single Responsibility (cada classe tem uma responsabilidade)
//
// Fluxo de autenticação:
//   LoginForm  AuthApiService.LoginAsync()  POST /api/auth/login
//    API valida credenciais  retorna cookie de sessão
//    AuthApiService retorna UserResponseDto
//    LoginForm armazena no SessionManager e abre MainForm
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;



namespace BetaFit.Desktop.Services
{
    /// <summary>
    /// Serviço de comunicação com os endpoints de autenticação da API
    /// </summary>
    public class AuthApiService
    {
        private readonly HttpClientHelper _http;

        public AuthApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        /// <summary>
        /// Realiza o login chamando POST /api/auth/login.
        ///
        /// O que acontece internamente:
        /// 1. Envia email + senha para a API em formato JSON
        /// 2. A API valida as credenciais com o ASP.NET Core Identity
        /// 3. Se válido, a API retorna um cookie de sessão + dados do usuário
        /// 4. O CookieContainer do HttpClient armazena o cookie automaticamente
        /// 5. Retornamos os dados do usuário para o LoginForm
        /// </summary>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="password">Senha do usuário</param>
        /// <returns>Tupla com sucesso, dados do usuário e mensagem de erro</returns>
        public async Task<(bool Success, UsersResponseDto? User, string ErrorMessage)>
            LoginAsync(string email, string password)
        {
            // Cria o objeto de requisição (DTO de login)
            var loginDto = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            // Envia para POST /api/auth/login
            var (success, data, error) = await _http.PostAsync<UsersResponseDto>(
                "/api/auth/login", loginDto);

            return (success, data, error);
        }

        /// <summary>
        /// Realiza o logout chamando POST /api/auth/logout.
        /// Também limpa os cookies de sessão localmente
        /// </summary>
        public async Task<(bool Success, string Error)> LogoutAsync()
        {
            var result = await _http.PostEmptyAsync("/api/auth/logout");

            _http.ClearCookies();

            return result;
        }

        /// <summary>
        /// Busca os dados do usuário autenticado via GET /api/auth/me.
        /// Útil para verificar se a sessão ainda está ativa
        /// </summary>
        public async Task<UsersResponseDto?> GetCurrentUserAsync()
        {
            return await _http.GetAsync<UsersResponseDto>("/api/auth/me");
        }

        /// <summary>
        /// Registra um novo usuário via POST /api/auth/register.
        /// </summary>
        public async Task<(bool Success, string Error)> RegisterAsync(
            string email, string password, string confirmPassword)
        {
            var dto = new RegisterRequestDto
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var (success, _, error) = await _http.PostAsync<object>("/api/auth/register", dto);
            return (success, error);
        }
    }
}
