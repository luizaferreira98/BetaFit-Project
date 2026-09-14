using System.Text.Json;
namespace BetaFit.Domain.Entities;

/// <summary>
/// Este código é uma classe auxiliar (helper) em C#
/// chamada ProductPhoto responsável por selecionar a URL da imagem ideal de um produto,
/// priorizando a cor escolhida pelo cliente e definindo fotos padrão caso a cor não possua 
/// imagens específicas.
/// </summary>
public static class ProductPhoto
{
    //Busca por galeria uma lista de fotos para a cor escolhida,
    //caso não encontre, busca por uma imagem padrão para a cor,
    //caso não encontre, retorna a primeira imagem do produto ou a imagem principal do produto.
    public static string? ForColor(Product product, string? color)
    {
        if (!string.IsNullOrWhiteSpace(color))
        {
            //Tenta desserializar a galeria de imagens do produto para a cor especificada.
            try
            {
                var galleries = JsonSerializer.Deserialize<Dictionary<string,List<string>>>(product.ColorGalleriesJson);
                var photo = galleries?.FirstOrDefault(x=>string.Equals(x.Key,color,StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(photo)) return photo;
                var images = JsonSerializer.Deserialize<Dictionary<string,string>>(product.ColorImageUrlsJson);
                photo = images?.FirstOrDefault(x=>string.Equals(x.Key,color,StringComparison.OrdinalIgnoreCase)).Value;
                if (!string.IsNullOrWhiteSpace(photo)) return photo;
            }
            //Se ocorrer um erro de desserialização, apenas ignora e continua a busca.
            catch (JsonException) { }
        }
        return product.Images.OrderBy(x=>x.SortOrder).FirstOrDefault()?.Url ?? product.ImageUrl;
    }
}
