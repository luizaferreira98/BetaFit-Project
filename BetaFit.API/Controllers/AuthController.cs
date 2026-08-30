// =============================================================================
// BetaFit.API - AuthController
// =============================================================================
//  CONCEITO IMPORTANTE: Autenticação na API
// Este controller implementa autenticação usando ASP.NET Core Identity
// com Cookie Authentication. NÃO há geração de token JWT aqui — o próprio
// Identity cuida do cookie de sessão.
//
// Endpoints:
// POST /api/auth/register  Registra um novo usuário
// POST /api/auth/login     Faz login (cria cookie de autenticação)
// POST /api/auth/logout    Faz logout (remove cookie)
// GET  /api/auth/me        Retorna dados do usuário autenticado
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BetaFit.Application.DTOs;

namespace BetaFit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //  CONCEITO: UserManager e SignInManager são serviços do Identity
        // UserManager: gerencia operações com usuários (criar, buscar, etc.)
        // SignInManager: gerencia operações de login/logout
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Registra um novo usuário.
        /// POST /api/auth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validação simples
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest(new { message = "As senhas não coincidem." });

            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            // Cria o usuário usando o UserManager
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                // Retorna os erros de validação do Identity
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Erro ao registrar.", errors });
            }

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        /// <summary>
        /// Faz login do usuário.
        /// POST /api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            // O campo "Email" do DTO representa o e-mail do Identity
            var result = await _signInManager.PasswordSignInAsync(
                dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Login ou senha inválidos." });

            // Busca o usuário para retornar seus dados
            var user = await _userManager.FindByEmailAsync(dto.Email);
            var roles = await _userManager.GetRolesAsync(user!);

            return Ok(new UserDto
            {
                Id = user!.Id,
                Email = user.Email!,
                Roles = roles
            });
        }

        /// <summary>
        /// Faz logout do usuário.
        /// POST /api/auth/logout
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout realizado com sucesso!" });
        }

        /// <summary>
        /// Retorna os dados do usuário autenticado.
        /// GET /api/auth/me
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized(new { message = "Usuário não autenticado." });

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            });
        }
    }
}