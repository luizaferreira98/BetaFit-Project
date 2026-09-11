using System.Security.Cryptography;
using System.Text;
namespace BetaFit.Application.Services;
public static class DemoBoleto
{
    public static string Create(decimal total)
    {
        var random=string.Concat(Enumerable.Range(0,25).Select(_=>RandomNumberGenerator.GetInt32(10)));
        return "0009"+"0"+"0000"+((long)decimal.Round(total*100)).ToString("D10")+random;
    }
    private static int Mod10(string digits) {var sum=0;var weight=2;foreach(var c in digits.Reverse()){var n=(c-'0')*weight;sum+=n/10+n%10;weight=3-weight;}return (10-sum%10)%10;}
    public static string Line(string code)
    {
        if(code.Length!=44)return "";
        var a=code[..4]+code.Substring(19,5);var b=code.Substring(24,10);var c=code.Substring(34,10);
        a+=Mod10(a);b+=Mod10(b);c+=Mod10(c);
        return $"{a[..5]}.{a[5..]} {b[..5]}.{b[5..]} {c[..5]}.{c[5..]} {code[4]} {code.Substring(5,14)}";
    }
    // Interleaved 2 of 5 encoding, generated offline as SVG. Input is strictly numeric.
    public static string Barcode(string digits)
    {
        if(digits.Length!=44 || digits.Any(c=>c<'0'||c>'9'))return "";
        string[] patterns={"00110","10001","01001","11000","00101","10100","01100","00011","10010","01010"};
        var bars=new StringBuilder();int x=10;
        void Part(int w,bool ink){if(ink)bars.Append($"<rect x='{x}' y='0' width='{w}' height='60'/>");x+=w;}
        Part(1,true);Part(1,false);Part(1,true);Part(1,false);
        for(int i=0;i<digits.Length;i+=2)for(int j=0;j<5;j++){Part(patterns[digits[i]-'0'][j]=='1'?3:1,true);Part(patterns[digits[i+1]-'0'][j]=='1'?3:1,false);}
        Part(3,true);Part(1,false);Part(1,true);
        return $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {x+10} 60' role='img' aria-label='Código de barras fictício, sem valor bancário' fill='black'>{bars}</svg>";
    }
}
