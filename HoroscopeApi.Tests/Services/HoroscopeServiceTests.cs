using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Implementations;
using HoroscopeApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace HoroscopeApi.Tests.Services;

public class HoroscopeServiceTests
{
    private static User SampleUser() => new()
    {
        Id = 1,
        Username = "marcelo",
        Email = "marcelo@test.com",
        PasswordHash = "hash",
        BirthDate = new DateOnly(1990, 10, 5) // Libra
    };

    [Fact]
    public async Task GetTodayAsync_CallsExternalApiOnlyOnce_WhenCalledTwice_DueToCache()
    {
        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(SampleUser());

        var queriesMock = new Mock<IHoroscopeQueryRepository>();
        queriesMock.Setup(r => r.AddAsync(It.IsAny<HoroscopeQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync((HoroscopeQuery q, CancellationToken _) => q);

        var clientMock = new Mock<INewAstroClient>();
        clientMock.Setup(c => c.GetHoroscopeAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync("Texto del horoscope");

        using var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new HoroscopeService(usersMock.Object, queriesMock.Object, clientMock.Object, cache);

        var first = await service.GetTodayAsync(1);
        var second = await service.GetTodayAsync(1);

        Assert.True(first.Success);
        Assert.True(second.Success);
        Assert.Equal("Libra", first.Data!.Sign);

        // La API externa se llama una sola vez (la segunda viene de cache)
        clientMock.Verify(c => c.GetHoroscopeAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        // El historial se registra en cada consulta (dos veces)
        queriesMock.Verify(r => r.AddAsync(It.IsAny<HoroscopeQuery>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetTodayAsync_WhenUserNotFound_ReturnsFail404()
    {
        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        var queriesMock = new Mock<IHoroscopeQueryRepository>();
        var clientMock = new Mock<INewAstroClient>();
        using var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new HoroscopeService(usersMock.Object, queriesMock.Object, clientMock.Object, cache);

        var result = await service.GetTodayAsync(99);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        clientMock.Verify(c => c.GetHoroscopeAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
