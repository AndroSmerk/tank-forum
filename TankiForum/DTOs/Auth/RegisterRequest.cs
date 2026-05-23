using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Auth;

/// <summary>
/// Registration request payload.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Unique username (3-50 characters).
    /// </summary>
    [Required, MinLength(3), MaxLength(50), RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, digits, and underscores.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Valid email address (max 100 characters).
    /// </summary>
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password (minimum 6 characters).
    /// </summary>
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
