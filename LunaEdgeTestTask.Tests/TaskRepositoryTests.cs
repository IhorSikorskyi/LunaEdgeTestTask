using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Tests
{
    public class TaskRepositoryTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;

        public TaskRepositoryTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTask_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var expectedTask = new Models.Task
            {
                Id = taskId,
                Title = "Test Task",
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = Status.Pending,
                Priority = Priority.Low
            };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(expectedTask);

            var result = await _taskRepositoryMock.Object.GetByIdAsync(taskId);

            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfTasks()
        {
            var tasks = new List<Models.Task>
            {
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 1", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Status = Status.Pending, Priority = Priority.Low },
                new Models.Task { Id = Guid.NewGuid(), Title = "Task 2", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Status = Status.InProgress, Priority = Priority.High }
            };

            _taskRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(tasks);

            var result = await _taskRepositoryMock.Object.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddAsync_CallsRepositoryAdd()
        {
            var newTask = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "New Task",
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = Status.Pending,
                Priority = Priority.Medium
            };

            _taskRepositoryMock.Setup(r => r.AddAsync(newTask))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _taskRepositoryMock.Object.AddAsync(newTask);

            _taskRepositoryMock.Verify(r => r.AddAsync(newTask), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepositoryUpdate()
        {
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Update Task",
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = Status.InProgress,
                Priority = Priority.High
            };

            _taskRepositoryMock.Setup(r => r.UpdateAsync(task))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _taskRepositoryMock.Object.UpdateAsync(task);

            _taskRepositoryMock.Verify(r => r.UpdateAsync(task), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDelete()
        {
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Delete Task",
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = Status.Completed,
                Priority = Priority.Low
            };

            _taskRepositoryMock.Setup(r => r.DeleteAsync(task))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _taskRepositoryMock.Object.DeleteAsync(task);

            _taskRepositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
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
                Tasks = new List<Models.Task>()
            };

            _taskRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            var result = await _taskRepositoryMock.Object.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task UserExistsAsync_ReturnsTrue_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            _taskRepositoryMock.Setup(r => r.UserExistsAsync(userId))
                .ReturnsAsync(true);

            var result = await _taskRepositoryMock.Object.UserExistsAsync(userId);

            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _taskRepositoryMock.Setup(r => r.UserExistsAsync(userId))
                .ReturnsAsync(false);

            var result = await _taskRepositoryMock.Object.UserExistsAsync(userId);

            Assert.False(result);
        }
    }
}