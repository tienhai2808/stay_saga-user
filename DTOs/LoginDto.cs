using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class LoginDto
{
  [Required(ErrorMessage = "Email is required")]
  [EmailAddress(ErrorMessage = "Email format is invalid")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "Password is required")]
  [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
  public string Password { get; set; } = string.Empty;
}
