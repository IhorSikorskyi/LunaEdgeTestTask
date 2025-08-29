using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using LunaEdgeTestTask.Services.Implementations;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Tests
{
    public class TasksServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly TasksService _tasksService;

        public TasksServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _tasksService = new TasksService(_taskRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTask_WhenValidRequest()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
             };
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Pending,
                Priority = Priority.Low
            };

            _taskRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _taskRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Models.Task>())).Returns(Task.CompletedTask);

            var result = await _tasksService.CreateTaskAsync(request, userId);

            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Status, result.Status);
            Assert.Equal(request.Priority, result.Priority);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldThrow_WhenTitleIsEmpty()
        {
            var userId = Guid.NewGuid();
            var request = new CreateTaskRequest
            {
                Title = "",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Pending,
                Priority = Priority.Low
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.CreateTaskAsync(request, userId));
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Pending,
                Priority = Priority.Low
            };

            _taskRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.CreateTaskAsync(request, userId));
        }

        [Fact]
        public async Task GetTasksAsync_ReturnsFilteredTasks()
        {
            var tasks = new List<Models.Task>
            {
                new Models.Task { Id = Guid.NewGuid(), Title = "Task1", Status = Status.Pending, Priority = Priority.Low, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Models.Task { Id = Guid.NewGuid(), Title = "Task2", Status = Status.Completed, Priority = Priority.High, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            var filterRequest = new GetTasksFilterRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Status = "Pending"
            };

            _taskRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            var result = await _tasksService.GetTasksAsync(filterRequest);

            Assert.Single(result);
            Assert.Equal("Task1", result[0].Title);
        }

        [Fact]
        public async Task GetTaskInfoAsync_ReturnsTaskInfo_WhenUserIsOwner()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Description = "Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                user = user
            };

            var request = new GetTaskInfoOrDeleteRequest { Id = task.Id };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

            var result = await _tasksService.GetTaskInfoAsync(request, userId);

            Assert.NotNull(result);
            Assert.Equal(task.Title, result.Title);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetTaskInfoAsync_ShouldThrow_WhenTaskNotFound()
        {
            var userId = Guid.NewGuid();
            var request = new GetTaskInfoOrDeleteRequest { Id = Guid.NewGuid() };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Models.Task)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.GetTaskInfoAsync(request, userId));
        }

        [Fact]
        public async Task GetTaskInfoAsync_ShouldThrow_WhenUserIsNotOwner()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Description = "Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = otherUserId,
                user = user
            };

            var request = new GetTaskInfoOrDeleteRequest { Id = task.Id };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.GetTaskInfoAsync(request, userId));
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesTask_WhenUserIsOwner()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                user = user
            };

            var updateRequest = new UpdateTaskRequest
            {
                TaskId = task.Id,
                Title = "New Title",
                Description = "New Desc",
                DueDate = DateTime.UtcNow.AddDays(2),
                Status = Status.Completed,
                Priority = Priority.High
            };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _taskRepositoryMock.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);

            var result = await _tasksService.UpdateTaskAsync(updateRequest, userId);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Desc", result.Description);
            Assert.Equal(Status.Completed, result.Status);
            Assert.Equal(Priority.High, result.Priority);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrow_WhenTaskNotFound()
        {
            var userId = Guid.NewGuid();
            var updateRequest = new UpdateTaskRequest
            {
                TaskId = Guid.NewGuid(),
                Title = "Title"
            };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(updateRequest.TaskId)).ReturnsAsync((Models.Task)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.UpdateTaskAsync(updateRequest, userId));
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrow_WhenUserIsNotOwner()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new User
            {
                Id = otherUserId,
                Username = "other",
                Email = "other@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = otherUserId,
                user = user
            };

            var updateRequest = new UpdateTaskRequest
            {
                TaskId = task.Id,
                Title = "New Title"
            };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

            await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.UpdateTaskAsync(updateRequest, userId));
        }

        [Fact]
        public async Task DeleteTaskAsync_DeletesTask_WhenUserIsOwner()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "user",
                Email = "user@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                user = user
            };

            var request = new GetTaskInfoOrDeleteRequest { Id = task.Id };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
            _taskRepositoryMock.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);

            var result = await _tasksService.DeleteTaskAsync(request, userId);

            Assert.True(result);
            _taskRepositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ReturnsFalse_WhenTaskNotFound()
        {
            var userId = Guid.NewGuid();
            var request = new GetTaskInfoOrDeleteRequest { Id = Guid.NewGuid() };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Models.Task)null);

            var result = await _tasksService.DeleteTaskAsync(request, userId);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ReturnsFalse_WhenUserIsNotOwner()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new User
            {
                Id = otherUserId,
                Username = "other",
                Email = "other@email.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                Tasks = new List<Models.Task>()
            };
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                Status = Status.Pending,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = otherUserId,
                user = user
            };

            var request = new GetTaskInfoOrDeleteRequest { Id = task.Id };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

            var result = await _tasksService.DeleteTaskAsync(request, userId);

            Assert.False(result);
        }
    }
}