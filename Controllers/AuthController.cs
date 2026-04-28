using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.DTOs;
using UserService.Services;
using Common.DTOs;

namespace UserService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var loginResponse = await _authService.RegisterAsync(dto);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
          loginResponse,
          "Register successful"
        );

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var loginResponse = await _authService.LoginAsync(dto);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
          loginResponse,
          "Login successful"
        );

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutDto dto)
    {
        await _authService.LogoutAsync(dto);
        var response = HttpApiResponseDto<object>.Success(
          null,
          "Logout successful"
        );

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
    {
        var refreshResponse = await _authService.RefreshTokenAsync(dto);
        var response = HttpApiResponseDto<AuthResponseDto>.Success(
          refreshResponse,
          "Token refresh successful"
        );

        return Ok(response);
    }

    [HttpGet("userinfo")]
    [Authorize]
    public async Task<IActionResult> UserInfo()
    {
        var keycloakId = User.FindFirstValue("sub");
        var userResponse = await _authService.UserInfoAsync(keycloakId ?? string.Empty);
        var response = HttpApiResponseDto<UserResponseDto>.Success(
          userResponse,
          "Get user info successful"
        );

        return Ok(response);
    }
}
