using System.ComponentModel.DataAnnotations;
namespace BetaFit.Application.DTOs;
public class CouponDto {
 public int Id {get;set;}
 [Required,RegularExpression("^[A-Za-z0-9_-]{3,40}$")] public string Code {get;set;}="";
 [Range(0.01,100)] public decimal Percent {get;set;}=10;
 [Range(0,999999)] public decimal Minimum {get;set;}
 public DateTime ExpiresAt {get;set;}=DateTime.UtcNow.AddDays(30);
 public bool Active {get;set;}=true;
 [Range(1,1000000)] public int MaxUses {get;set;}=100;
 public int Used {get;set;}
}
