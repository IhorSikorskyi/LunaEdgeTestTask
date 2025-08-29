using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Implementations;

public class TaskRepository(LETestTaskContext dbContext) : ITaskRepository
{
    // Get a task by its ID including the related user
    public async Task<Models.Task?> GetByIdAsync(Guid id)
    {
        return await dbContext.Tasks
            .Include(t => t.user)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    // Get all tasks including their related users (read-only, no tracking)
    public async Task<List<Models.Task>> GetAllAsync()
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.user)
            .ToListAsync();
    }

    // Add a new task to the database
    public async Task AddAsync(Models.Task task)
    {
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
    }

    // Update an existing task in the database
    public async Task UpdateAsync(Models.Task task)
    {
        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync();
    }

    // Delete a task from the database
    public async Task DeleteAsync(Models.Task task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
    }

    // Get a user by their ID
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId);
    }

    // Check if a user exists by ID
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(u => u.Id == userId);
    }
}