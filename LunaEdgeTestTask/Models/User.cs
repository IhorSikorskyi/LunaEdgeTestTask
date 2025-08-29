using System.ComponentModel.DataAnnotations;

namespace LunaEdgeTestTask.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public required string Email { get; set; } = null!;

    [Required]
    public required string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public required string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(7);

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}