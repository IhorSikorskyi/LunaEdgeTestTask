using LunaEdgeTestTask.Models;
using LunaEdgeTestTask.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace LunaEdgeTestTask.Repositories.Implementations;

public class UserRepository(LETestTaskContext dbContext) : IUserRepository
{
    // Get a user by their ID
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId);
    }

    // Get a user by username or email
    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    // Check if a user exists by ID
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await dbContext.Users.AnyAsync(u => u.Id == userId);
    }

    // Add a new user to the database
    public async Task AddAsync(User user)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }
}