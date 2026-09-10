using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace BetaFit.Application.Services;
public static class ReviewPolicy {
 static string Normalize(string s)=>new string(s.ToLowerInvariant().Normalize(NormalizationForm.FormD).Where(c=>CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark).ToArray()).Replace('0','o').Replace('1','i').Replace('3','e').Replace('4','a').Replace('@','a').Replace('$','s');
 public static string? Validate(string? text,IEnumerable<string>? extra=null){var s=(text??"").Trim();if(s.Length>500||s.Count(char.IsLetterOrDigit)<10||s.Where(char.IsLetter).Select(char.ToLowerInvariant).Distinct().Count()<4)return "Escreva de 10 a 500 caracteres, contando sua experiência com palavras.";var normalized=Normalize(s);var words=Regex.Matches(normalized,"[a-z]+").Select(m=>m.Value).ToHashSet();var blocked=new[]{"porra","caralho","puta","puto","merda","foder","foda","cacete","buceta","cuzao","fdp","vtnc","pqp"}.Concat(extra??Array.Empty<string>());if(blocked.Any(w=>words.Contains(Normalize(w))))return "Revise o comentário: há uma palavra não permitida.";return null;}
}
