using LunaEdgeTestTask.Models;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<bool> UserExistsAsync(Guid userId);
    Task AddAsync(User user);
}