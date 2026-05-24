using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Chat;

public class SendChatMessageRequest
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
