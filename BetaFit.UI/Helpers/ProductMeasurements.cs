using BetaFit.Application.DTOs;
namespace BetaFit.UI.Helpers;
public static class ProductMeasurements
{
    public static Dictionary<string,Dictionary<string,decimal>> For(ProductDto p)
    {
        var result=new Dictionary<string,Dictionary<string,decimal>>(StringComparer.OrdinalIgnoreCase);
        string[] standard={"PP","P","M","G","GG","XG","Único"};
        var lower=p.CategoryName.Contains("Short",StringComparison.OrdinalIgnoreCase)||p.CategoryName.Contains("Legging",StringComparison.OrdinalIgnoreCase)||p.CategoryName.Contains("Calça",StringComparison.OrdinalIgnoreCase);
        foreach(var size in p.AvailableSizes)
        {
            if(p.SizeMeasurements.TryGetValue(size,out var data)&&data.Count>0){result[size]=data;continue;}
            var i=Math.Max(0,Array.IndexOf(standard,size));
            if(int.TryParse(size,out var shoe))result[size]=new(){{"Comprimento do pé",20m+(shoe-30)*0.67m}};
            else if(lower)result[size]=new(){{"Cintura (circunferência)",64+4*i},{"Quadril (circunferência)",88+4*i},{"Comprimento",p.CategoryName.Contains("Short")?34+2*i:92+2*i}};
            else result[size]=new(){{"Comprimento frente",67+2*i},{"Comprimento da manga",20+i},{"Comprimento costas",70+2*i},{"Tórax (circunferência)",103+4*i}};
        }
        return result;
    }
}
