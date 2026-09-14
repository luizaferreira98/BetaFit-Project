namespace BetaFit.Domain.Entities;

public class Favorite
{
    public int Id { get; set; }

    // ID do usuário que favoritou o produto
    public string UserId { get; set; } = string.Empty;
    
    // ID do produto favorito
    public int ProductId { get; set; }

    //Referenciando com o DTO de produtos
    public Product? Product { get; set; }
}
