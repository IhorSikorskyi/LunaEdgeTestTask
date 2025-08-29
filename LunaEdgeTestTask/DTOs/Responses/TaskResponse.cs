using LunaEdgeTestTask.Models;

namespace LunaEdgeTestTask.DTOs.Responses;

public class CreateTaskResponse
{
    public string Title { get; set; } = null!;
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class FilteredTasksResponse
{
    public string Title { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class GetTaskInfoResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = String.Empty;
    public DateTime? DueDate { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Username { get; set; } = null!;
}

public class UpdateTaskResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
}