using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
