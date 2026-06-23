using HoroscopeApi.DataAccess;
using HoroscopeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _set;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _set.ToListAsync(cancellationToken);

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _set.FindAsync([id], cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _set.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _set.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
