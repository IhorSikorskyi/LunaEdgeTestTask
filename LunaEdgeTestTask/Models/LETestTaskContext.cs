using Microsoft.EntityFrameworkCore;

namespace LunaEdgeTestTask.Models;

public class LETestTaskContext: DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;

    public LETestTaskContext(DbContextOptions<LETestTaskContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region User

        // Unique constraints for Username
        modelBuilder.Entity<User>() 
            .HasIndex(u => u.Username)
            .IsUnique();

        // Unique constraints for Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        #endregion

        #region Task

        // Enum to string conversion for Status
        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<string>();

        // Enum to string conversion for Priority
        modelBuilder.Entity<Task>()
            .Property(t => t.Priority)
            .HasConversion<string>();

        #endregion
    }

    public override int SaveChanges() // Overriding SaveChanges to automatically update timestamps
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is User &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((User)entry.Entity).CreatedAt = DateTime.UtcNow; // set CreatedAt only when adding
            }

            ((User)entry.Entity).UpdatedAt = DateTime.UtcNow; // always update UpdatedAt
        }

        return base.SaveChanges();
    }
}