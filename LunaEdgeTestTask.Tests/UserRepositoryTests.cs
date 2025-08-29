using System;
using System.Threading.Tasks;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Tests
{
    public class UserRepositoryTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserRepositoryTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var expectedUser = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tasks = []
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            var result = await _userRepositoryMock.Object.GetByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            var result = await _userRepositoryMock.Object.GetByIdAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameOrEmailAsync_ReturnsUser_WhenMatchExists()
        {
            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = "hash",
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tasks = []
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync("user1"))
                .ReturnsAsync(expectedUser);

            var result = await _userRepositoryMock.Object.GetByUsernameOrEmailAsync("user1");

            Assert.NotNull(result);
            Assert.Equal("user1", result.Username);
        }

        [Fact]
        public async Task GetByUsernameOrEmailAsync_ReturnsNull_WhenNoMatch()
        {
            _userRepositoryMock.Setup(r => r.GetByUsernameOrEmailAsync("notfound"))
                .ReturnsAsync((User)null);

            var result = await _userRepositoryMock.Object.GetByUsernameOrEmailAsync("notfound");

            Assert.Null(result);
        }

        [Fact]
        public async Task UserExistsAsync_ReturnsTrue_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(r => r.UserExistsAsync(userId))
                .ReturnsAsync(true);

            var result = await _userRepositoryMock.Object.UserExistsAsync(userId);

            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(r => r.UserExistsAsync(userId))
                .ReturnsAsync(false);

            var result = await _userRepositoryMock.Object.UserExistsAsync(userId);

            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_CallsRepositoryAdd()
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "newuser",
                Email = "newuser@example.com",
                PasswordHash = "hash",
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tasks = []
            };

            _userRepositoryMock.Setup(r => r.AddAsync(newUser))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _userRepositoryMock.Object.AddAsync(newUser);

            _userRepositoryMock.Verify(r => r.AddAsync(newUser), Times.Once);
        }
    }
}