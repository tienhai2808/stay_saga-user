using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository(AppDbContext db)
{
  private readonly AppDbContext _db = db;

  public async Task<User> CreateAsync(User user)
  {
    _db.Users.Add(user);
    await _db.SaveChangesAsync();
    return user;
  }

  public async Task<bool> ExistsByEmailAsync(string email)
  {
    return await _db.Users.AnyAsync(u => u.Email == email);
  }

  public async Task<User?> GetByIdAsync(long id)
  {
    return await _db.Users.FindAsync(id);
  }
}
