using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using UsersAPI.Models.DTOs;
using UsersAPI.Models.Entities;
using UsersAPI.Repositories;

namespace UsersAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _userRepository.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _userRepository.FindByIdAsync(id);

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            // Check for duplicate Email
            var existingUser = await _userRepository.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"A user with the email '{dto.Email}' already exists.");
            }

            // Optional: Check for duplicate Username if your logic requires it unique
            var existingUsername = await _userRepository.FindByUsernameAsync(dto.Username);
            if (existingUsername != null)
            {
                throw new InvalidOperationException($"The username '{dto.Username}' is already taken.");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null) return false;

            // Validate email uniqueness if the email is being changed
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailExists = await _userRepository.FindByEmailAsync(dto.Email);
                if (emailExists != null)
                {
                    throw new InvalidOperationException($"Cannot update to email '{dto.Email}' because it is already in use.");
                }
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.Username)) user.Username = dto.Username;

            await _userRepository.UpdateAsync(user);
            return true;
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }

        private string HashPassword(string password)
        {
            // 1. Generate a unique Salt
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // 2. Configure Argon2id
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, // Number of threads to use
                Iterations = 4,         // Number of passes
                MemorySize = 1024 * 64   // 64 MB of RAM
            };

            // 3. Get the Hash
            var hash = argon2.GetBytes(32);

            // 4. Combine Salt and Hash for storage (usually in Base64)
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 64
            };

            var newHash = argon2.GetBytes(32);
            return newHash.SequenceEqual(hash);
        }

    }

}
