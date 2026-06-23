using HoroscopeApi.DataAccess;
using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await _set.AnyAsync(u => u.Username == username, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _set.AnyAsync(u => u.Email == email, cancellationToken);
}
