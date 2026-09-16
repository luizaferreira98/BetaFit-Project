using System.ComponentModel.DataAnnotations;
namespace BetaFit.Domain.Entities;
public class SupportConversation
{
    public int Id { get; set; }
    [MaxLength(450)] public string UserId { get; set; } = "";
    [MaxLength(160)] public string Subject { get; set; } = "";
    public int? ProductId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<SupportMessage> Messages { get; set; } = new();
}
public class SupportMessage
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public SupportConversation Conversation { get; set; } = null!;
    [MaxLength(1000)] public string Text { get; set; } = "";
    public bool IsStaff { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
