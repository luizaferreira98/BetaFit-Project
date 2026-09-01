using System.Security.Claims;
using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers
{
    [ApiController, Authorize, Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        public ProfileController(UserManager<IdentityUser> userManager) => _userManager = userManager;

        [HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            return user is null ? Unauthorized() : Ok(await Map(user));
        }

        [HttpPut]
        public async Task<ActionResult<UserDto>> Update(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.BirthDate.Date > DateTime.Today.AddYears(-18)) return BadRequest(new { message = "É necessário ter 18 anos ou mais." });
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            if (!string.Equals(user.Email, dto.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var emailResult = await _userManager.SetEmailAsync(user, dto.Email.Trim());
                if (!emailResult.Succeeded) return BadRequest(new { message = string.Join(" ", emailResult.Errors.Select(e => e.Description)) });
                user.UserName = dto.Email.Trim();
                var nameResult = await _userManager.UpdateAsync(user);
                if (!nameResult.Succeeded) return BadRequest(new { message = string.Join(" ", nameResult.Errors.Select(e => e.Description)) });
            }

            user.PhoneNumber = dto.PhoneNumber.Trim();
            var phoneResult = await _userManager.UpdateAsync(user);
            if (!phoneResult.Succeeded) return BadRequest(new { message = string.Join(" ", phoneResult.Errors.Select(e => e.Description)) });

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (dto.NewPassword != dto.ConfirmNewPassword) return BadRequest(new { message = "A nova senha e a confirmação não coincidem." });
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword)) return BadRequest(new { message = "Informe a senha atual para trocar a senha." });
                var passwordResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passwordResult.Succeeded) return BadRequest(new { message = string.Join(" ", passwordResult.Errors.Select(e => e.Description)) });
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var oldName = claims.FirstOrDefault(c => c.Type == "FullName");
            var oldBirth = claims.FirstOrDefault(c => c.Type == "BirthDate");
            var remove = new List<Claim>();
            if (oldName != null) remove.Add(oldName);
            if (oldBirth != null) remove.Add(oldBirth);
            if (remove.Count > 0) await _userManager.RemoveClaimsAsync(user, remove);
            await _userManager.AddClaimsAsync(user, new[] { new Claim("FullName", dto.FullName.Trim()), new Claim("BirthDate", dto.BirthDate.ToString("yyyy-MM-dd")) });

            return Ok(await Map(user));
        }

        private async Task<UserDto> Map(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            DateTime? birth = DateTime.TryParse(claims.FirstOrDefault(c => c.Type == "BirthDate")?.Value, out var b) ? b : null;
            return new UserDto
            { Id = user.Id, Email = user.Email ?? string.Empty, FullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? user.UserName ?? string.Empty, PhoneNumber = user.PhoneNumber ?? string.Empty, BirthDate = birth, Roles = await _userManager.GetRolesAsync(user) };
        }
    }
}
