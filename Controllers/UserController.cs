using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.DTOs;
using UserDomainService = UserService.Services.UserService;

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
    var response = new ApiResponse<AuthResponseDto>(
      StatusCodes.Status200OK,
      "Register successful",
      loginResponse
    );

    return Ok(response);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto dto)
  {
    var loginResponse = await _userService.LoginAsync(dto);
    var response = new ApiResponse<AuthResponseDto>(
      StatusCodes.Status200OK,
      "Login successful",
      loginResponse
    );

    return Ok(response);
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout(LogoutDto dto)
  {
    await _userService.LogoutAsync(dto);
    var response = new ApiResponse<object>(
      StatusCodes.Status200OK,
      "Logout successful",
      null
    );

    return Ok(response);
  }

  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
  {
    var refreshResponse = await _userService.RefreshTokenAsync(dto);
    var response = new ApiResponse<AuthResponseDto>(
      StatusCodes.Status200OK,
      "Token refresh successful",
      refreshResponse
    );

    return Ok(response);
  }

  [HttpGet("me")]
  [Authorize]
  public async Task<IActionResult> GetMe()
  {
    var keycloakId = User.FindFirstValue("sub");
    var userResponse = await _userService.UserInfoAsync(keycloakId ?? string.Empty);
    var response = new ApiResponse<UserResponseDto>(
      StatusCodes.Status200OK,
      "Get user info successful",
      userResponse
    );

    return Ok(response);
  }
}
