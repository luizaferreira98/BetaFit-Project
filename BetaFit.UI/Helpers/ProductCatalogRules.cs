namespace BetaFit.UI.Helpers;

public static class ProductCatalogRules
{
    private static readonly string[] ShoeCategories = { "tênis", "tenis", "calçado", "calcado", "calçados", "calcados", "sapato", "sapatos" };
    private static readonly string[] ClothingCategories = { "camiseta", "camisetas", "regata", "regatas", "calça", "calcas", "calças", "legging", "leggings", "short", "shorts", "bermuda", "bermudas", "top", "tops", "jaqueta", "jaquetas", "moletom", "moletons", "blusa", "blusas", "casaco", "casacos", "vestido", "vestidos", "macacão", "macacoes", "macacões", "body", "conjunto", "conjuntos", "roupa", "roupas" };

    public static bool IsShoeCategory(string? categoryName) => !string.IsNullOrWhiteSpace(categoryName) && ShoeCategories.Any(x => categoryName.Trim().Contains(x, StringComparison.OrdinalIgnoreCase));
    public static bool RequiresSize(string? categoryName) => IsShoeCategory(categoryName) || (!string.IsNullOrWhiteSpace(categoryName) && ClothingCategories.Any(x => categoryName.Trim().Contains(x, StringComparison.OrdinalIgnoreCase)));
    public static List<string> NormalizeSizes(string? categoryName, IEnumerable<string>? sizes)
    {
        var values = (sizes ?? Enumerable.Empty<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (IsShoeCategory(categoryName)) return values.Where(s => int.TryParse(s, out var n) && n >= 20 && n <= 55).OrderBy(int.Parse).Select(x => x.ToString()).ToList();
        if (!RequiresSize(categoryName)) return new();
        return values.Where(s => s.Length <= 8 && !int.TryParse(s, out _)).ToList();
    }
}
