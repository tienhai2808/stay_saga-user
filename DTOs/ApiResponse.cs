namespace UserService.DTOs;

public sealed record ApiResponse<T>(
  int StatusCode,
  string Message,
  T? Data
);
