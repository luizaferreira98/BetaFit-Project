using System.ComponentModel.DataAnnotations;
namespace BetaFit.Application.DTOs;
public class SupportCreateDto
{
    [Required, StringLength(160, MinimumLength=3)] public string Subject { get; set; } = "";
    [Required, StringLength(1000, MinimumLength=1)] public string Text { get; set; } = "";
    public int? ProductId { get; set; }
}
public class SupportSendDto
{
    [Required, StringLength(1000, MinimumLength=1)] public string Text { get; set; } = "";
}
public class SupportThreadDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = "";
    public int? ProductId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SupportMessageDto> Messages { get; set; } = new();
}
public class SupportMessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsStaff { get; set; }
    public DateTime CreatedAt { get; set; }
}
