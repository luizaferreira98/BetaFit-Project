using System.ComponentModel.DataAnnotations;
namespace BetaFit.Application.DTOs;
public sealed class HalfStarAttribute : ValidationAttribute
{
    public HalfStarAttribute() { ErrorMessage = "Escolha de 0,5 a 5 estrelas, em intervalos de meia estrela."; }
    public override bool IsValid(object? value) => value is decimal rating && rating >= .5m && rating <= 5m && rating * 2 == decimal.Truncate(rating * 2);
}
