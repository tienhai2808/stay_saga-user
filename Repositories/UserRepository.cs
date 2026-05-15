using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> FindByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId, cancellationToken);
    }
}
