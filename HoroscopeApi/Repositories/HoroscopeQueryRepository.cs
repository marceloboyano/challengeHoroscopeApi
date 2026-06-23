using HoroscopeApi.DataAccess;
using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.Repositories;

public class HoroscopeQueryRepository : RepositoryBase<HoroscopeQuery>, IHoroscopeQueryRepository
{
    public HoroscopeQueryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<HoroscopeQuery> items, int totalCount)> GetByUserPagedAsync(
        int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var baseQuery = _set.Where(q => q.UserId == userId);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<(string Sign, int Count)>> GetSignStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _set
            .GroupBy(q => q.Sign)
            .Select(g => new { Sign = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        return stats.Select(x => (x.Sign, x.Count));
    }
}
