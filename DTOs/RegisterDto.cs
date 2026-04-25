using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required")]
    [MinLength(2, ErrorMessage = "FirstName must be at least 2 characters long")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required")]
    [MinLength(2, ErrorMessage = "LastName must be at least 2 characters long")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Phone format is invalid")]
    public string Phone { get; set; } = string.Empty;
}
