using LunaEdgeTestTask.Models;

namespace LunaEdgeTestTask.DTOs.Requests;

public class CreateTaskRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }

    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class GetTasksFilterRequest
{
    // Optional parameters for pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Optional parameters for filtering
    public string? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public int? Priority { get; set; }

    // Optional parameters for sorting
    public string? SortBy { get; set; }
    public bool Description { get; set; } = false;
}

public class GetTaskInfoOrDeleteRequest
{
    public Guid Id { get; set; }
}

public class UpdateTaskRequest
{
    public Guid TaskId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
}