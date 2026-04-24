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
    var userResponse = await _userService.RegisterAsync(dto);
    var response = new ApiResponse<UserResponseDto>(
      StatusCodes.Status201Created,
      "Register user thành công",
      userResponse
    );

    return Created($"/users/{userResponse.Id}", response);
  }
}
