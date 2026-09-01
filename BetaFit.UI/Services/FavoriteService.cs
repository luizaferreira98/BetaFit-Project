using System.Text.Json;
namespace BetaFit.UI.Services
{
    public static class FavoriteService
    {
        private const string Key="betafit_favorites";
        public static HashSet<int> Get(HttpContext c){try{return JsonSerializer.Deserialize<HashSet<int>>(c.Session.GetString(Key)??"[]")??new();}catch{return new();}}
        private static void Save(HttpContext c,HashSet<int> ids)=>c.Session.SetString(Key,JsonSerializer.Serialize(ids));
        public static bool Toggle(HttpContext c,int id){var ids=Get(c);var added=ids.Add(id);if(!added)ids.Remove(id);Save(c,ids);return added;}
        public static bool Contains(HttpContext c,int id)=>Get(c).Contains(id);
        public static void Remove(HttpContext c,int id){var ids=Get(c);ids.Remove(id);Save(c,ids);}
        public static void Clear(HttpContext c)=>c.Session.Remove(Key);
    }
}
