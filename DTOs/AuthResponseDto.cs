namespace UserService.DTOs;

public sealed record AuthResponseDto(
  string AccessToken,
  string RefreshToken,
  int ExpiresIn,
  int RefreshExpiresIn
);
