using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    [MinLength(8, ErrorMessage = "Password phải có ít nhất 8 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FullName là bắt buộc")]
    [MinLength(2, ErrorMessage = "FullName phải có ít nhất 2 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone là bắt buộc")]
    [Phone(ErrorMessage = "Phone không đúng định dạng")]
    public string Phone { get; set; } = string.Empty;
}
