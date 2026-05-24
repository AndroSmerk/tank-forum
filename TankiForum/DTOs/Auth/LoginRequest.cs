using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Auth;

/// <summary>
/// Login request payload.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Username or email address.
    /// </summary>
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// Account password.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
