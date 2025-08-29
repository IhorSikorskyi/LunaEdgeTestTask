using LunaEdgeTestTask.DTOs.Requests;
using LunaEdgeTestTask.DTOs.Responses;
using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories;
using LunaEdgeTestTask.Repositories.Interfaces;
using LunaEdgeTestTask.Services.Interfaces;

namespace LunaEdgeTestTask.Services.Implementations;

public class TasksService(ITaskRepository taskRepository) : ITasksService
{
    // Create a new task for a given user
    public async Task<CreateTaskResponse> CreateTaskAsync(CreateTaskRequest request, Guid userId)
    {
        // Validate title
        if (string.IsNullOrEmpty(request.Title) || request.Title.Length > 50)
            throw new ArgumentException("Title is required and must be 50 characters or fewer.");

        // Get the user or throw if not found
        var user = await taskRepository.GetUserByIdAsync(userId)
                   ?? throw new ArgumentException("User not found.");

        // Create a new Task entity
        var newTask = new Models.Task
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            DueDate = request.DueDate,
            Status = request.Status,
            Priority = request.Priority,
            UserId = userId,
            user = user
        };

        // Save the task
        await taskRepository.AddAsync(newTask);

        // Return response DTO
        return new CreateTaskResponse
        {
            Title = newTask.Title,
            Status = newTask.Status,
            Priority = newTask.Priority
        };
    }

    // Get all tasks with filtering, sorting, and pagination
    public async Task<List<FilteredTasksResponse>> GetTasksAsync(GetTasksFilterRequest request)
    {
        // Ensure valid pagination parameters
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageSize > 100) request.PageSize = 100;

        // Fetch all tasks
        var tasks = await taskRepository.GetAllAsync();
        var query = tasks.AsQueryable();

        // --- Filtering ---

        // Filter by Status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<Status>(request.Status, out var statusEnum))
            query = query.Where(t => t.Status == statusEnum);

        // Filter by DueDate
        if (request.DueDate.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == request.DueDate.Value.Date);

        // Filter by Priority
        if (request.Priority.HasValue && Enum.IsDefined(typeof(Priority), request.Priority.Value))
        {
            var priorityEnum = (Priority)request.Priority.Value;
            query = query.Where(t => t.Priority == priorityEnum);
        }

        // --- Sorting ---

        // Default sorting is by CreatedAt
        query = request.SortBy switch
        {
            "DueDate" => request.Description ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
            "Priority" => request.Description ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            _ => request.Description ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };

        // --- Pagination ---
        var result = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new FilteredTasksResponse
            {
                Title = t.Title,
                DueDate = t.DueDate,
                Status = t.Status,
                Priority = t.Priority
            })
            .ToList();

        return result;
    }

    // Get detailed information about a task (only if user is the owner)
    public async Task<GetTaskInfoResponse> GetTaskInfoAsync(GetTaskInfoOrDeleteRequest request, Guid userId)
    {
        // Ensure the task exists
        var task = await taskRepository.GetByIdAsync(request.Id)
                   ?? throw new ArgumentException("Task not found.");

        // Check if user is the owner
        if (task.UserId != userId)
            throw new ArgumentException("You do not have permission to view this task.");

        // Return task details
        return new GetTaskInfoResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Username = task.user.Username
        };
    }

    // Update an existing task (only if user is the owner)
    public async Task<UpdateTaskResponse> UpdateTaskAsync(UpdateTaskRequest updateRequest, Guid userId)
    {
        // Ensure the task exists
        var task = await taskRepository.GetByIdAsync(updateRequest.TaskId)
                   ?? throw new ArgumentException("Task not found.");

        // Check if user is the owner
        if (task.UserId != userId)
            throw new ArgumentException("You do not have permission to update this task.");

        // Update fields if provided
        if (!string.IsNullOrEmpty(updateRequest.Title))
        {
            if (updateRequest.Title.Length > 50)
                throw new ArgumentException("Title must be 50 characters or fewer.");

            task.Title = updateRequest.Title;
            task.Description = updateRequest.Description;
            task.DueDate = updateRequest.DueDate;
            task.Status = updateRequest.Status ?? task.Status;
            task.Priority = updateRequest.Priority ?? task.Priority;
        }

        // Save changes
        await taskRepository.UpdateAsync(task);

        // Return response DTO
        return new UpdateTaskResponse
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority
        };
    }

    // Delete a task (only if user is the owner)
    public async Task<bool> DeleteTaskAsync(GetTaskInfoOrDeleteRequest request, Guid userId)
    {
        // Ensure the task exists
        var task = await taskRepository.GetByIdAsync(request.Id);

        // If not found or not owned by the user → return false
        if (task == null || task.UserId != userId)
            return false;

        // Delete and return success
        await taskRepository.DeleteAsync(task);
        return true;
    }
}
