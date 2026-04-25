using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByKeycloakIdAsync(string keycloakId)
    {
        return await _db.Users
          .AsNoTracking()
          .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId);
    }
}
