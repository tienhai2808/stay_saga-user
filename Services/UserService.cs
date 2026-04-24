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

  public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
  {
    var exists = await _userRepo.ExistsByEmailAsync(dto.Email);
    if (exists)
      throw new ConflictException("Email đã tồn tại");

    var keycloakId = await _keycloakProvider.CreateUserAsync(dto.Email, dto.Password, dto.FullName);

    var user = new User
    {
      Id = _idGenerator.CreateId(),
      Email = dto.Email,
      FullName = dto.FullName,
      Phone = dto.Phone,
      KeycloakId = keycloakId
    };

    await _userRepo.CreateAsync(user);
    return new UserResponseDto(
      user.Id,
      user.Email,
      user.FullName,
      user.Phone
    );
  }
}
