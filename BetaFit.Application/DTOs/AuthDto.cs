using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required, StringLength(120, MinimumLength = 3)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime BirthDate { get; set; }
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Required] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateProfileDto
    {
        [Required, StringLength(120, MinimumLength = 3)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime BirthDate { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
