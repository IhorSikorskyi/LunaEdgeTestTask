using System;
using System.Threading.Tasks;
using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using LunaEdgeTestTask.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // For ConfigurationBinder.GetValue<T>(...)
            _configurationMock.Setup(c => c["AppSettings:AccessToken"]).Returns("supersecretkey123456789012345678901234");
            _configurationMock.Setup(c => c["AppSettings:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["AppSettings:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["AppSettings:TokenExpiryMinutes"]).Returns("60");
            _configurationMock.Setup(c => c["AppSettings:RefreshTokenExpiryDays"]).Returns("7");

            // For GetSection("AppSettings")["..."]
            var appSettingsSectionMock = new Mock<IConfigurationSection>();
            appSettingsSectionMock.Setup(s => s["AccessToken"]).Returns("supersecretkey123456789012345678901234");
            appSettingsSectionMock.Setup(s => s["Issuer"]).Returns("TestIssuer");
            appSettingsSectionMock.Setup(s => s["Audience"]).Returns("TestAudience");
            appSettingsSectionMock.Setup(s => s["TokenExpiryMinutes"]).Returns("60");
            appSettingsSectionMock.Setup(s => s["RefreshTokenExpiryDays"]).Returns("7");
            _configurationMock.Setup(c => c.GetSection("AppSettings")).Returns(appSettingsSectionMock.Object);

            // For ConfigurationBinder.GetValue<T>(...) internal calls
            _configurationMock.Setup(c => c.GetSection("AppSettings:AccessToken").Value).Returns("supersecretkey123456789012345678901234");
            _configurationMock.Setup(c => c.GetSection("AppSettings:Issuer").Value).Returns("TestIssuer");
            _configurationMock.Setup(c => c.GetSection("AppSettings:Audience").Value).Returns("TestAudience");
            _configurationMock.Setup(c => c.GetSection("AppSettings:TokenExpiryMinutes").Value).Returns("60");
            _configurationMock.Setup(c => c.GetSection("AppSettings:RefreshTokenExpiryDays").Value).Returns("7");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenValidRequest()
        {
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUserExists()
        {
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = "hashedpassword",
                    RefreshToken = "sometoken"
                });

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenPasswordsDoNotMatch()
        {
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password2!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenPasswordNotComplex()
        {
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "simple",
                ConfirmPassword = "simple"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUsernameIsEmpty()
        {
            var request = new RegisterRequest
            {
                Username = "",
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>().HashPassword(null, "Password1!"),
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            var request = new LoginRequest
            {
                UsernameOrEmail = "testuser",
                Password = "Password1!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            var result = await _authService.LoginAsync(request);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            var request = new LoginRequest
            {
                UsernameOrEmail = "notfound",
                Password = "Password1!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordIncorrect()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>().HashPassword(null, "Password1!"),
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            var request = new LoginRequest
            {
                UsernameOrEmail = "testuser",
                Password = "WrongPassword!"
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordIsEmpty()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<User>().HashPassword(null, "Password1!"),
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            var request = new LoginRequest
            {
                UsernameOrEmail = "testuser",
                Password = ""
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnToken_WhenValidRefreshToken()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                RefreshToken = "validtoken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };

            var request = new RefreshTokenRequest
            {
                UserId = userId,
                AccessToken = "validtoken"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var result = await _authService.RefreshTokenAsync(request);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrow_WhenInvalidRefreshToken()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                RefreshToken = "othertoken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
            };

            var request = new RefreshTokenRequest
            {
                UserId = userId,
                AccessToken = "invalidtoken"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RefreshTokenAsync(request));
        }
    }
}