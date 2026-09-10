using System.Security.Claims;
using System.Text.Json;
using BetaFit.Application.DTOs;
namespace BetaFit.Application.Services;
public static class DemoWallet
{
    public static List<SavedCardDto> Read(IEnumerable<Claim> claims)
    {
        var list=new List<SavedCardDto>();
        foreach(var c in claims.Where(c=>c.Type=="DemoCard")) { try { var card=JsonSerializer.Deserialize<SavedCardDto>(c.Value); if(card!=null)list.Add(card); } catch(JsonException) {} }
        var last=claims.FirstOrDefault(c=>c.Type=="Card.Last4")?.Value;
        if(!string.IsNullOrEmpty(last))list.Insert(0,new SavedCardDto{Id="legacy",Last4=last,Holder=claims.FirstOrDefault(c=>c.Type=="Card.Holder")?.Value??"",Brand=claims.FirstOrDefault(c=>c.Type=="Card.Brand")?.Value??"",Expiry=claims.FirstOrDefault(c=>c.Type=="Card.Expiry")?.Value??""});
        return list;
    }
    public static bool ValidExpiry(string value) => DateTime.TryParseExact(value,"MM/yy",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out var date) && date.AddMonths(1)>DateTime.Today;
}
