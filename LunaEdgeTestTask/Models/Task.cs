using System.ComponentModel.DataAnnotations;

namespace LunaEdgeTestTask.Models;

public class Task
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Title { get; set; } = null!;
    public string? Description { get; set; } = String.Empty;
    public DateTime? DueDate { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to User
    public Guid UserId { get; set; }
    public User user { get; set; } = null!;
}

public enum Status
{
    Pending,
    InProgress,
    Completed
}

public enum Priority
{
    Low,
    Medium,
    High
}