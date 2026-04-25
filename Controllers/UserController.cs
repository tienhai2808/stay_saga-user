using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.DTOs;
using UserDomainService = UserService.Services.UserService;
using Common.Response;

namespace UserService.Controllers;

[ApiController]
[Route("users")]
public class UserController(UserDomainService userService) : ControllerBase
{
    private readonly UserDomainService _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var loginResponse = await _userService.RegisterAsync(dto);
        var response = HttpApiResponse<AuthResponseDto>.Success(
          loginResponse,
          "Register successful"
        );

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var loginResponse = await _userService.LoginAsync(dto);
        var response = HttpApiResponse<AuthResponseDto>.Success(
          loginResponse,
          "Login successful"
        );

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutDto dto)
    {
        await _userService.LogoutAsync(dto);
        var response = HttpApiResponse<object>.Success(
          null,
          "Logout successful"
        );

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
    {
        var refreshResponse = await _userService.RefreshTokenAsync(dto);
        var response = HttpApiResponse<AuthResponseDto>.Success(
          refreshResponse,
          "Token refresh successful"
        );

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var keycloakId = User.FindFirstValue("sub");
        var userResponse = await _userService.UserInfoAsync(keycloakId ?? string.Empty);
        var response = HttpApiResponse<UserResponseDto>.Success(
          userResponse,
          "Get user info successful"
        );

        return Ok(response);
    }
}
