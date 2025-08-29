using LunaEdgeTestTask.Models;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<Models.Task?> GetByIdAsync(Guid id);
    Task<List<Models.Task>> GetAllAsync();
    Task AddAsync(Models.Task task);
    Task UpdateAsync(Models.Task task);
    Task DeleteAsync(Models.Task task);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<bool> UserExistsAsync(Guid userId);
}