using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class CreateReviewDto
{
    [Range(1, 5)] public int Rating { get; set; }
    [Required, StringLength(500, MinimumLength=10)] public string Comment { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
}

public class ReviewDto
{
    public string ModerationStatus { get; set; } = "Pendente";
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
