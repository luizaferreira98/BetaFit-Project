using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class ForgotPasswordDto
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required, StringLength(500)] public string Token { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)] public string NewPassword { get; set; } = string.Empty;
    [Required, StringLength(100)] public string ConfirmPassword { get; set; } = string.Empty;
}

public class ConfirmProfileChangeDto
{
    [Required, StringLength(500)] public string Token { get; set; } = string.Empty;
}

public class ProfileChangeResponseDto
{
    public bool RequiresVerification { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}

public class RequestEmailChangeDto
{
    [Required,EmailAddress,StringLength(256)] public string Email { get; set; } = "";
    [Required,StringLength(100)] public string CurrentPassword { get; set; } = "";
}
