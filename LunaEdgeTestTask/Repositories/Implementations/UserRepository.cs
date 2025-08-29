using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Implementations;

public class UserRepository(LETestTaskContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task AddAsync(User user)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }
}