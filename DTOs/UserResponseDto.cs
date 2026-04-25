namespace UserService.DTOs;

public sealed record UserResponseDto(
  string Id,
  string Email,
  string FirstName,
  string LastName,
  string Phone
);
