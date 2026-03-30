using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.Entities;
using UsersAPI.Data;


namespace UsersAPI.Repositories
{
    public class UserRepository : BaseRepository<User, DatabaseContext>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context) { }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }
    }

}
