using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Messages;

public class SendMessageRequest
{
    [Required]
    public int ReceiverId { get; set; }

    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
