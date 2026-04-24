namespace UserService.DTOs;

public sealed record UserResponseDto(
  long Id,
  string Email,
  string FullName,
  string Phone
);
