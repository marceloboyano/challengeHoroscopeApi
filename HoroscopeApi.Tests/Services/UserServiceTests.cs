using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Implementations;
using Moq;

namespace HoroscopeApi.Tests.Services;

public class UserServiceTests
{
    private static User SampleUser() => new()
    {
        Id = 1,
        Username = "marcelo",
        Email = "marcelo@test.com",
        PasswordHash = "hash",
        BirthDate = new DateOnly(1990, 10, 5)
    };

    [Fact]
    public async Task UpdateProfileAsync_WhenNoRealChange_DoesNotPersist()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(SampleUser());

        var service = new UserService(users.Object);

        // Mismo email que ya tiene y misma fecha => no hay cambios reales
        var request = new UpdateProfileRequest
        {
            Email = "marcelo@test.com",
            BirthDate = new DateOnly(1990, 10, 5)
        };

        var result = await service.UpdateProfileAsync(1, request);

        Assert.True(result.Success);
        users.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_WhenBirthDateChanges_Persists()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(SampleUser());

        var service = new UserService(users.Object);

        var request = new UpdateProfileRequest { BirthDate = new DateOnly(1995, 1, 1) };

        var result = await service.UpdateProfileAsync(1, request);

        Assert.True(result.Success);
        users.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WhenUserNotFound_Returns404()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var service = new UserService(users.Object);

        var result = await service.UpdateProfileAsync(99, new UpdateProfileRequest { Email = "x@test.com" });

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        users.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
