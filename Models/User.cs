namespace UserService.Models;

public class User
{
  public long Id { get; set; }
  public string KeycloakId { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
}