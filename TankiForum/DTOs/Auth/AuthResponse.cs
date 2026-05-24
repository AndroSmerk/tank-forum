namespace TankiForum.DTOs.Auth;

/// <summary>
/// Authentication response returned after successful login or registration.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT bearer token for authenticated requests.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Username of the authenticated user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier of the authenticated user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User role: "User" or "Admin".
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
