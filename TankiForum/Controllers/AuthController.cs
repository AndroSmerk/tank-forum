using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Auth;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

/// <summary>
/// Handles user registration, login, and logout operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">Registration details (username, email, password).</param>
    /// <returns>JWT token and user info.</returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Invalid input or username/email already taken.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Log in with username/email and password.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>JWT token and user info.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Log out the current user (stateless — client should discard the token).
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>
    /// Request password reset. Returns a reset token in development mode.
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await _authService.ForgotPasswordAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Reset password using a reset token.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request);
        return Ok(new { message = "Password reset successfully." });
    }
}
