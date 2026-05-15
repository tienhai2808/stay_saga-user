using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.DTOs;
using UserService.Services;
using Common.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Common.Exceptions;

namespace UserService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
    {
        var loginResponse = await _authService.RegisterAsync(dto, cancellationToken);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
            loginResponse,
            "Register successful"
        );

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var loginResponse = await _authService.LoginAsync(dto, cancellationToken);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
            loginResponse,
            "Login successful"
        );

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutDto dto, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(dto, cancellationToken);
        var response = HttpApiResponseDto<object>.Success(
            null,
            "Logout successful"
        );

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var refreshResponse = await _authService.RefreshTokenAsync(dto, cancellationToken);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
            refreshResponse,
            "Token refresh successful"
        );

        return Ok(response);
    }

    [HttpGet("userinfo")]
    [Authorize]
    public async Task<IActionResult> UserInfo(CancellationToken cancellationToken)
    {
        var keycloakId = GetCurrentKeycloakId();
        var userResponse = await _authService.UserInfoAsync(keycloakId, cancellationToken);
        var response = HttpApiResponseDto<UserResponseDto>.Success(
            userResponse,
            "Get user info successful"
        );

        return Ok(response);
    }

    private string GetCurrentKeycloakId()
    {
        var keycloakId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedException("Invalid access token");
        return keycloakId;
    }
}
