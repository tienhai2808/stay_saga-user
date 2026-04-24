using Microsoft.AspNetCore.Mvc;
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
}
