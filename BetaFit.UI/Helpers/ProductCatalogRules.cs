namespace BetaFit.UI.Helpers;

public static class ProductCatalogRules
{
    private static readonly string[] SizeCategories =
    {
        "camiseta", "camisetas", "regata", "regatas", "calça", "calcas", "calças",
        "legging", "leggings", "short", "shorts", "bermuda", "bermudas", "top", "tops",
        "jaqueta", "jaquetas", "moletom", "moletons", "blusa", "blusas", "casaco", "casacos",
        "vestido", "vestidos", "macacão", "macacoes", "macacões", "body", "conjunto",
        "conjuntos", "roupa", "roupas", "tênis", "tenis", "calçado", "calcados", "calçados"
    };

    public static bool RequiresSize(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return false;
        var normalized = categoryName.Trim().ToLowerInvariant();
        return SizeCategories.Any(normalized.Contains);
    }

    public static List<string> NormalizeSizes(string? categoryName, IEnumerable<string>? sizes)
    {
        if (!RequiresSize(categoryName)) return new List<string>();
        return (sizes ?? Enumerable.Empty<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
