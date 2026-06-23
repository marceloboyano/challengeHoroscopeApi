using HoroscopeApi.Entities;

namespace HoroscopeApi.Repositories.Interfaces;

public interface IHoroscopeQueryRepository : IRepositoryBase<HoroscopeQuery>
{
    Task<(IEnumerable<HoroscopeQuery> items, int totalCount)> GetByUserPagedAsync(
        int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<IEnumerable<(string Sign, int Count)>> GetSignStatsAsync(CancellationToken cancellationToken = default);
}
