// =============================================================================
// BetaFit.UI - AccountController (Proxy HTTP para a BetaFit.API)
// =============================================================================
//  CONCEITO: A autenticação real (senhas, hashes, roles) vive inteiramente
using Microsoft.AspNetCore.Http;
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
                ModelState.AddModelError(string.Empty, await ReadApiErrorAsync(response, "Não foi possível criar a conta. Verifique os dados e tente outro e-mail."));
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

            Response.Cookies.Delete(".AspNetCore.Identity.Application");
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
            if (!ModelState.IsValid) return View("Profile", new UserDto { FullName=dto.FullName, Email=dto.Email, PhoneNumber=dto.PhoneNumber, BirthDate=dto.BirthDate, Cpf=dto.Cpf, Cep=dto.Cep, Street=dto.Street, Number=dto.Number, Complement=dto.Complement, Neighborhood=dto.Neighborhood, City=dto.City, State=dto.State, Roles=User.Claims.Where(c=>c.Type==ClaimTypes.Role).Select(c=>c.Value).ToList() });
            var response=await _apiClient.PutAsJsonAsync("api/profile",dto);
            if(!response.IsSuccessStatusCode){var error=await response.Content.ReadFromJsonAsync<ApiErrorDto>();ModelState.AddModelError(string.Empty,error?.Message??"Não foi possível atualizar o perfil.");return View("Profile",ToProfileViewModel(dto));}
            var result=await response.Content.ReadFromJsonAsync<ProfileChangeResponseDto>();
            if(result?.RequiresVerification==true)
            {
                TempData["Sucesso"]=result.Message;
                return RedirectToAction(nameof(Profile));
            }
            if(result?.User!=null) await RefreshLocalIdentityAsync(result.User);
            TempData["Sucesso"]=result?.Message??"Perfil atualizado com sucesso.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost("UpdateEmail"), ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateEmail(string email, string currentPassword)
        {
            var profile = await GetProfileAsync(); if (profile is null) return RedirectToAction(nameof(Login));
            if (string.IsNullOrWhiteSpace(email) || email.Length > 256 || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
                return await ProfileErrorAsync(profile, "Informe um e-mail válido.");
            return await SubmitProfileChangeAsync(profile, email.Trim(), currentPassword, null, null);
        }

        [HttpPost("UpdatePassword"), ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            var profile = await GetProfileAsync(); if (profile is null) return RedirectToAction(nameof(Login));
            if (newPassword != confirmNewPassword) return await ProfileErrorAsync(profile, "A nova senha e a confirmação não coincidem.");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length is < 6 or > 100) return await ProfileErrorAsync(profile, "A nova senha deve ter entre 6 e 100 caracteres.");
            return await SubmitProfileChangeAsync(profile, profile.Email, currentPassword, newPassword, confirmNewPassword);
        }

        [HttpPost("UpdateAddress"), ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateAddress(CheckoutAddressDto dto)
        {
            var profile = await GetProfileAsync(); if (profile is null) return RedirectToAction(nameof(Login));
            if (!ModelState.IsValid) return await ProfileErrorAsync(profile, "Revise os campos do endereço.");
            var response = await _apiClient.PutAsJsonAsync("api/profile/checkout-address", dto);
            if (!response.IsSuccessStatusCode) return await ProfileErrorAsync(profile, await ReadApiErrorAsync(response, "Não foi possível atualizar o endereço."));
            TempData["Sucesso"] = "Endereço atualizado com sucesso."; return RedirectToAction(nameof(Profile));
        }

        [HttpPost("UpdateCard"), ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateCard(PaymentCardDto dto)
        {
            var profile = await GetProfileAsync(); if (profile is null) return RedirectToAction(nameof(Login));
            dto.CardNumber = new string((dto.CardNumber ?? string.Empty).Where(char.IsDigit).ToArray());
            dto.SecurityCode = new string((dto.SecurityCode ?? string.Empty).Where(char.IsDigit).ToArray());
            if (!ModelState.IsValid) return await ProfileErrorAsync(profile, "Revise os dados do cartão demonstrativo.");
            var response = await _apiClient.PutAsJsonAsync("api/profile/card", dto);
            if (!response.IsSuccessStatusCode) return await ProfileErrorAsync(profile, await ReadApiErrorAsync(response, "Não foi possível salvar o cartão."));
            TempData["Sucesso"] = "Cartão demonstrativo cadastrado. Apenas bandeira, final e validade foram armazenados."; return RedirectToAction(nameof(Profile));
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword() => View(new ForgotPasswordDto());

        [HttpPost("ForgotPassword"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _authClient.PostAsJsonAsync("api/password/forgot", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorDto>();
                ModelState.AddModelError(string.Empty, error?.Message ?? "Não foi possível enviar o e-mail.");
                return View(dto);
            }
            TempData["Sucesso"] = "Se o e-mail estiver cadastrado, enviamos as instruções de recuperação.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string? email, string? token) => View(new ResetPasswordDto { Email = email ?? string.Empty, Token = token ?? string.Empty });

        [HttpPost("ResetPassword"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _authClient.PostAsJsonAsync("api/password/reset", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorDto>();
                ModelState.AddModelError(string.Empty, error?.Message ?? "Não foi possível redefinir a senha.");
                return View(dto);
            }
            TempData["Sucesso"] = "Senha redefinida com sucesso. Faça login com a nova senha.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("ConfirmProfileChange")]
        public async Task<IActionResult> ConfirmProfileChange(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return View("ConfirmationResult", "Link inválido ou expirado.");
            var response = await _authClient.PostAsJsonAsync("api/profile/confirm-change", new ConfirmProfileChangeDto { Token = token });
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorDto>();
                return View("ConfirmationResult", error?.Message ?? "Link inválido ou expirado.");
            }
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("ConfirmationResult", "Alterações confirmadas. Por segurança, entre novamente na sua conta.");
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
                    var cookieParts = apiCookieString.Split('=', 2);
                    if (cookieParts.Length == 2)
                    {
                        Response.Cookies.Append(cookieParts[0], cookieParts[1], new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = Request.IsHttps,
                            SameSite = SameSiteMode.Lax,
                            Path = "/"
                        });
                    }
                }
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userDto.Id),
                new(ClaimTypes.Name, userDto.Email),
                new(ClaimTypes.Email, userDto.Email),
                new("ApiCookie", apiCookieString)
            };
            if (!string.IsNullOrWhiteSpace(userDto.FullName)) claims.Add(new Claim("FullName", userDto.FullName));
            if (!string.IsNullOrWhiteSpace(userDto.Cpf)) claims.Add(new Claim("Cpf", userDto.Cpf));
            if (!string.IsNullOrWhiteSpace(userDto.City)) claims.Add(new Claim("City", userDto.City));

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

        private UserDto ToProfileViewModel(UpdateProfileDto dto) => new()
        {
            FullName=dto.FullName, Email=dto.Email, PhoneNumber=dto.PhoneNumber, BirthDate=dto.BirthDate,
            Cpf=dto.Cpf, Cep=dto.Cep, Street=dto.Street, Number=dto.Number, Complement=dto.Complement,
            Neighborhood=dto.Neighborhood, City=dto.City, State=dto.State,
            Roles=User.Claims.Where(c=>c.Type==ClaimTypes.Role).Select(c=>c.Value).ToList()
        };

        private async Task<UserDto?> GetProfileAsync()
        {
            var response = await _apiClient.GetAsync("api/profile");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
        }

        private async Task<IActionResult> SubmitProfileChangeAsync(UserDto profile, string email, string currentPassword, string? newPassword, string? confirmNewPassword)
        {
            var dto = new UpdateProfileDto { FullName=profile.FullName, Email=email, PhoneNumber=profile.PhoneNumber, BirthDate=profile.BirthDate ?? DateTime.Today.AddYears(-18), Cpf=profile.Cpf, Cep=profile.Cep, Street=profile.Street, Number=profile.Number, Complement=profile.Complement, Neighborhood=profile.Neighborhood, City=profile.City, State=profile.State, CurrentPassword=currentPassword, NewPassword=newPassword, ConfirmNewPassword=confirmNewPassword };
            var response = await _apiClient.PutAsJsonAsync("api/profile", dto);
            if (!response.IsSuccessStatusCode) return await ProfileErrorAsync(profile, await ReadApiErrorAsync(response, "Não foi possível atualizar a conta."));
            var result = await response.Content.ReadFromJsonAsync<ProfileChangeResponseDto>();
            TempData["Sucesso"] = result?.Message ?? "Solicitação registrada."; return RedirectToAction(nameof(Profile));
        }

        private Task<IActionResult> ProfileErrorAsync(UserDto profile, string message)
        {
            ViewData["Title"] = "Meu perfil"; ModelState.Clear(); ModelState.AddModelError(string.Empty, message); return Task.FromResult<IActionResult>(View("Profile", profile));
        }

        private static async Task<string> ReadApiErrorAsync(HttpResponseMessage response, string fallback)
        {
            try { var error=await response.Content.ReadFromJsonAsync<ApiErrorDto>(); return string.IsNullOrWhiteSpace(error?.Message)?fallback:error.Message; }
            catch { return fallback; }
        }

        private sealed class ApiErrorDto { public string Message { get; set; } = string.Empty; }
    }
}
