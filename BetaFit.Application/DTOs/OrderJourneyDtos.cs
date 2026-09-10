using System.ComponentModel.DataAnnotations;
namespace BetaFit.Application.DTOs;
public class OrderMessageDto{public string Text{get;set;}="";public bool IsStaff{get;set;}public DateTime CreatedAt{get;set;}}
public class OrderExperienceDto{[Range(1,5)]public int Rating{get;set;}[Required,StringLength(500,MinimumLength=10)]public string Comment{get;set;}="";}
public class TrackingDto{[Required,StringLength(80)]public string Code{get;set;}="";[Required,StringLength(250)]public string Description{get;set;}="";}
