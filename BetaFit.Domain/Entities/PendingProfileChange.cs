namespace BetaFit.Domain.Entities;

public class PendingProfileChange //Alteracao de perfil pendente, aguardando confirmacao do usuario via email.
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Tentativas de alteração de perfil, caso o usuário não confirme a alteração dentro do prazo.
    public int Attempts { get; set; }

    // Data e hora em que a solicitação de alteração de perfil foi feita.
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    //Token de confirmação da alteração de perfil, enviado por email para o usuário.
    public string TokenHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Cep { get; set; }
    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }

    // Bairro do endereço do usuário, caso seja fornecido. Exemplo: "Centro", "Jardim América", etc.
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public DateTime BirthDate { get; set; }
    public string? NewPasswordHash { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class ProductReview
{

    // Status da moderação da avaliação do produto. Pode ser "Pendente", "Aprovada" ou "Rejeitada".
    public string ModerationStatus { get; set; } = "Pendente";
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    //Lista de link de imagens em formato JSON, caso o usuário tenha enviado fotos junto com a avaliação do produto. Exemplo: ["https://example.com/photo1.jpg", "https://example.com/photo2.jpg"].
    public string PhotoUrlsJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
