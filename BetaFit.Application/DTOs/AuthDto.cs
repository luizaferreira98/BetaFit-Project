using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required, StringLength(120, MinimumLength = 3)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
        [Required, Phone, StringLength(15, MinimumLength = 10)] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime BirthDate { get; set; }
        [Required, StringLength(30)] public string Gender { get; set; } = string.Empty;
        [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = string.Empty;
        [Required, StringLength(100)] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Cpf { get; set; }
        public string? Cep { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? CardHolderName { get; set; }
        public string? CardBrand { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardExpiry { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateProfileDto
    {
        [Required, StringLength(120, MinimumLength = 3)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
        [Required, Phone, StringLength(15, MinimumLength = 10)] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime BirthDate { get; set; }
        [StringLength(14)] public string? Cpf { get; set; }
        [StringLength(9)] public string? Cep { get; set; }
        [StringLength(180)] public string? Street { get; set; }
        [StringLength(20)] public string? Number { get; set; }
        [StringLength(120)] public string? Complement { get; set; }
        [StringLength(120)] public string? Neighborhood { get; set; }
        [StringLength(120)] public string? City { get; set; }
        [StringLength(2)] public string? State { get; set; }
        [StringLength(100)] public string? CurrentPassword { get; set; }
        [StringLength(100, MinimumLength = 6)] public string? NewPassword { get; set; }
        [StringLength(100)] public string? ConfirmNewPassword { get; set; }
    }

    public class CheckoutAddressDto
    {
        [Required, RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$")] public string Cpf { get; set; } = string.Empty;
        [Required, RegularExpression(@"^\d{5}-?\d{3}$")] public string Cep { get; set; } = string.Empty;
        [Required, StringLength(180)] public string Street { get; set; } = string.Empty;
        [Required, StringLength(20)] public string Number { get; set; } = string.Empty;
        [StringLength(120)] public string? Complement { get; set; }
        [Required, StringLength(120)] public string Neighborhood { get; set; } = string.Empty;
        [Required, StringLength(120)] public string City { get; set; } = string.Empty;
        [Required, StringLength(2, MinimumLength = 2)] public string State { get; set; } = string.Empty;
    }

    public class PaymentCardDto
    {
        [Required, StringLength(120, MinimumLength = 3)] public string CardHolderName { get; set; } = string.Empty;
        [Required, RegularExpression(@"^\d{13,19}$", ErrorMessage = "Informe um número de cartão válido.")]
        public string CardNumber { get; set; } = string.Empty;
        [Required, RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Use o formato MM/AA.")]
        public string Expiry { get; set; } = string.Empty;
        [Required, RegularExpression(@"^\d{3,4}$", ErrorMessage = "Informe o código de segurança.")]
        public string SecurityCode { get; set; } = string.Empty;
    }
}
