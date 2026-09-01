// =============================================================================
// BetaFit.UI - AccountController (Proxy HTTP para a BetaFit.API)
// =============================================================================
//  CONCEITO: A autenticação real (senhas, hashes, roles) vive inteiramente
// na BetaFit.API via ASP.NET Core Identity. Este Controller apenas repassa
// as credenciais para a API e, com a resposta dela, cria um cookie de
// sessão LOCAL do MVC (para proteger as páginas com [Authorize]) guardando
// também o cookie da API para ser reenviado nas próximas chamadas.
// =============================================================================

using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly HttpClient _authClient;
        private readonly HttpClient _apiClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            // Login/Register usam o cliente SEM o cookie interceptador:
            // é justamente aqui que o cookie da API é obtido pela 1ª vez.
            _authClient = httpClientFactory.CreateClient("ApiClientAuth");
            _apiClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["Title"] = "Entrar";
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            ViewData["Title"] = "Entrar";
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(dto);

            var signedIn = await SignInWithApiAsync(dto);
            if (!signedIn)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(dto);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            ViewData["Title"] = "Criar conta";
            return View(new RegisterDto());
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            ViewData["Title"] = "Criar conta";

            if (!ModelState.IsValid)
                return View(dto);

            if (dto.Password != dto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "As senhas não coincidem.");
                return View(dto);
            }

            var response = await _authClient.PostAsJsonAsync("api/Auth/register", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível criar a conta. Verifique os dados e tente outro e-mail.");
                return View(dto);
            }

            // Loga o usuário automaticamente logo após o cadastro
            var signedIn = await SignInWithApiAsync(new LoginDto { Email = dto.Email, Password = dto.Password });
            if (!signedIn)
                return RedirectToAction(nameof(Login));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _apiClient.PostAsync("api/Auth/logout", null);
            }
            catch (HttpRequestException)
            {
                // Mesmo se a API estiver fora do ar, ainda removemos o cookie local.
            }

            // O carrinho é vinculado à sessão do navegador, não à conta.
            // Limpá-lo no logout evita que outra pessoa usando o mesmo navegador
            // veja itens deixados pela conta anterior.
            CartService.Clear(HttpContext);
            FavoriteService.Clear(HttpContext);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile()
        {
            var response = await _apiClient.GetAsync("api/profile");
            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Login));
            var profile = await response.Content.ReadFromJsonAsync<UserDto>();
            ViewData["Title"]="Meu perfil";
            return View("Profile", profile);
        }

        [HttpPost("Profile"), ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile(UpdateProfileDto dto)
        {
            ViewData["Title"]="Meu perfil";
            if (!ModelState.IsValid) return View("Profile", dto);
            var response=await _apiClient.PutAsJsonAsync("api/profile",dto);
            if(!response.IsSuccessStatusCode){var error=await response.Content.ReadFromJsonAsync<ApiErrorDto>();ModelState.AddModelError(string.Empty,error?.Message??"Não foi possível atualizar o perfil.");return View("Profile",dto);}
            var updated=await response.Content.ReadFromJsonAsync<UserDto>();
            if(updated!=null)
            {
                if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                    await SignInWithApiAsync(new LoginDto { Email = updated.Email, Password = dto.NewPassword });
                else
                    await RefreshLocalIdentityAsync(updated);
            }
            TempData["Sucesso"]="Perfil atualizado com sucesso.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Acesso negado";
            return View();
        }

        /// <summary>
        /// Chama POST /api/Auth/login na BetaFit.API, guarda o cookie de
        /// autenticação retornado numa Claim e cria o cookie local do MVC.
        /// </summary>
        private async Task<bool> SignInWithApiAsync(LoginDto dto)
        {
            var response = await _authClient.PostAsJsonAsync("api/Auth/login", dto);
            if (!response.IsSuccessStatusCode)
                return false;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            if (userDto is null)
                return false;

            var apiCookieString = string.Empty;
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var identityCookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Identity.Application="));
                if (!string.IsNullOrEmpty(identityCookie))
                {
                    apiCookieString = identityCookie.Split(';')[0];
                }
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userDto.Id),
                new(ClaimTypes.Name, userDto.Email),
                new(ClaimTypes.Email, userDto.Email),
                new("ApiCookie", apiCookieString)
            };
            if (!string.IsNullOrWhiteSpace(userDto.FullName))
                claims.Add(new Claim("FullName", userDto.FullName));

            foreach (var role in userDto.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return true;
        }
        private async Task RefreshLocalIdentityAsync(UserDto user)
        {
            var claims=new List<Claim>{new(ClaimTypes.NameIdentifier,user.Id),new(ClaimTypes.Name,user.Email),new(ClaimTypes.Email,user.Email)};
            if(!string.IsNullOrWhiteSpace(user.FullName))claims.Add(new Claim("FullName",user.FullName));
            claims.AddRange(User.Claims.Where(c=>c.Type==ClaimTypes.Role || c.Type=="ApiCookie"));
            var identity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));
        }

        private sealed class ApiErrorDto { public string Message { get; set; } = string.Empty; }
    }
}
