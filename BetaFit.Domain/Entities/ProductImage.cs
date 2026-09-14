namespace BetaFit.Domain.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    //Url do produto, que pode ser um link para uma imagem armazenada em um serviço de hospedagem de imagens ou em um servidor próprio.
    public string Url { get; set; } = string.Empty;

    // Indica a ordem de exibição da imagem em relação a outras imagens do mesmo produto. Um valor menor indica que a imagem deve ser exibida antes de imagens com valores maiores.
    public int SortOrder { get; set; }

    // Indica se a imagem é a principal do produto. A imagem principal é geralmente exibida em destaque na página do produto e em listas de produtos. (define a posicao exata da imagem principal)
    public bool IsPrimary { get; set; }

    //Referenciando com o DTO de produtos
    public Product? Product { get; set; }
}
