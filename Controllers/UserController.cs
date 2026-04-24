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
}
