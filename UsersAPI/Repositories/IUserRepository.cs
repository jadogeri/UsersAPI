using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.Entities;


namespace UsersAPI.Repositories
{
    public interface IUserRepository
    {
        // Common CRUD (matches BaseRepository signatures)
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> FindByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);

        // User-Specific Operations
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUsernameAsync(string username);
    }

}
