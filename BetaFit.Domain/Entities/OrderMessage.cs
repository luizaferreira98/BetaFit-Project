namespace BetaFit.Domain.Entities;
public class OrderMessage {public int Id{get;set;}public int OrderId{get;set;}public string UserId{get;set;}="";public bool IsStaff{get;set;}public string Text{get;set;}="";public DateTime CreatedAt{get;set;}=DateTime.UtcNow;}
