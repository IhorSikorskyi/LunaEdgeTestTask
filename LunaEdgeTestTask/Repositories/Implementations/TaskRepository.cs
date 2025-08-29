using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Implementations;

public class TaskRepository(LETestTaskContext dbContext) : ITaskRepository
{
    public async Task<Models.Task?> GetByIdAsync(Guid id)
    {
        return await dbContext.Tasks
            .Include(t => t.user)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Models.Task>> GetAllAsync()
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.user)
            .ToListAsync();
    }

    public async Task AddAsync(Models.Task task)
    {
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Task task)
    {
        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Models.Task task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId);
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(u => u.Id == userId);
    }
}