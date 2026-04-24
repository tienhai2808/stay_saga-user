namespace UserService.DTOs;

public sealed record UserResponseDto(
  long Id,
  string Email,
  string FirstName,
  string LastName,
  string Phone
);
