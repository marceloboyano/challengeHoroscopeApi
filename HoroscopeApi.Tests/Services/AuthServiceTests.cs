using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Implementations;
using HoroscopeApi.Services.Interfaces;
using Moq;

namespace HoroscopeApi.Tests.Services;

public class AuthServiceTests
{
    private static Mock<ITokenService> TokenServiceMock()
    {
        var mock = new Mock<ITokenService>();
        mock.Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns(("fake-jwt-token", DateTime.UtcNow.AddHours(1)));
        return mock;
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ReturnsConflict()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.ExistsByUsernameAsync("marcelo", It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        var service = new AuthService(users.Object, TokenServiceMock().Object);

        var request = new RegisterRequest { Username = "marcelo", Email = "m@test.com", BirthDate = new DateOnly(1990, 10, 5), Password = "Passw0rd!" };
        var result = await service.RegisterAsync(request);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        users.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_ReturnsCreatedWithToken()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        users.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((User u, CancellationToken _) => u);

        var service = new AuthService(users.Object, TokenServiceMock().Object);

        var request = new RegisterRequest { Username = "nuevo", Email = "nuevo@test.com", BirthDate = new DateOnly(1995, 3, 25), Password = "Passw0rd!" };
        var result = await service.RegisterAsync(request);

        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("fake-jwt-token", result.Data!.Token);
        users.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsUnauthorized()
    {
        var user = new User
        {
            Id = 1,
            Username = "marcelo",
            Email = "m@test.com",
            BirthDate = new DateOnly(1990, 10, 5),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Passw0rd!")
        };

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByUsernameAsync("marcelo", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var service = new AuthService(users.Object, TokenServiceMock().Object);

        var result = await service.LoginAsync(new LoginRequest { Username = "marcelo", Password = "incorrecta" });

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectPassword_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            Username = "marcelo",
            Email = "m@test.com",
            BirthDate = new DateOnly(1990, 10, 5),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Passw0rd!")
        };

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByUsernameAsync("marcelo", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var service = new AuthService(users.Object, TokenServiceMock().Object);

        var result = await service.LoginAsync(new LoginRequest { Username = "marcelo", Password = "Passw0rd!" });

        Assert.True(result.Success);
        Assert.Equal("fake-jwt-token", result.Data!.Token);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsUnauthorized()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var service = new AuthService(users.Object, TokenServiceMock().Object);

        var result = await service.LoginAsync(new LoginRequest { Username = "noexiste", Password = "x" });

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }
}
