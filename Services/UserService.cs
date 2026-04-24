using IdGen;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Models;
using UserService.Providers;
using UserService.Repositories;

namespace UserService.Services;

public class UserService(UserRepository userRepo, KeycloakProvider keycloakProvider, IIdGenerator<long> idGenerator)
{
  private readonly UserRepository _userRepo = userRepo;
  private readonly KeycloakProvider _keycloakProvider = keycloakProvider;
  private readonly IIdGenerator<long> _idGenerator = idGenerator;

  public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
  {
    var exists = await _userRepo.ExistsByEmailAsync(dto.Email);
    if (exists)
      throw new ConflictException("Email already exists");

    var keycloakId = await _keycloakProvider.CreateUserAsync(dto.Email, dto.Password, dto.FirstName, dto.LastName);

    var user = new User
    {
      Id = _idGenerator.CreateId(),
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      Phone = dto.Phone,
      KeycloakId = keycloakId
    };

    await _userRepo.CreateAsync(user);
    return await _keycloakProvider.LoginAsync(dto.Email, dto.Password);
  }

  public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
  {
    return await _keycloakProvider.LoginAsync(dto.Email, dto.Password);
  }

  public async Task LogoutAsync(LogoutDto dto)
  {
    await _keycloakProvider.LogoutAsync(dto.RefreshToken);
  }

  public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
  {
    return await _keycloakProvider.RefreshTokenAsync(dto.RefreshToken);
  }

  public async Task<UserResponseDto> UserInfoAsync(string keycloakId)
  {
    if (string.IsNullOrWhiteSpace(keycloakId))
      throw new UnauthorizedException("Invalid access token");

    var user = await _userRepo.GetByKeycloakIdAsync(keycloakId) ?? throw new NotFoundException("User not found");

    return new UserResponseDto(
      user.Id,
      user.Email,
      user.FirstName,
      user.LastName,
      user.Phone
    );
  }
}
