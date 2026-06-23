using HoroscopeApi.Entities;

namespace HoroscopeApi.Repositories.Interfaces;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
