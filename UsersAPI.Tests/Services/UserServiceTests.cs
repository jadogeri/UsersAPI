using Moq;
using NUnit.Framework;
using UsersAPI.Models.DTOs;
using UsersAPI.Models.Entities;
using UsersAPI.Repositories;
using UsersAPI.Services;

namespace UsersAPI.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepo;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object);
        }

        #region Happy Path
        [Test, Category("HappyPath")]
        public async Task CreateUserAsync_ShouldReturnUser_WhenDataIsValid()
        {
            var dto = new CreateUserDto { Username = "newuser", Email = "test@test.com", Password = "Password123!" };
            _mockRepo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null!);

            var result = await _userService.CreateUserAsync(dto);

            Assert.Multiple(() => {
                Assert.That(result.Username, Is.EqualTo(dto.Username));
                Assert.That(result.PasswordHash, Does.Contain(".")); // Verify salt.hash format
            });
        }

        [Test, Category("HappyPath")]
        public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
        {
            var password = "SecurePass123";
            var dto = new CreateUserDto { Password = password, Email = "email", Username = "username" };
            // We use the service's own hashing to create a valid stored hash for the test
            var user = new User { PasswordHash = ReflectionHash(password) };

            var isValid = _userService.VerifyPassword(password, user.PasswordHash);
            Assert.That(isValid, Is.True);
        }

        [Test, Category("HappyPath")]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
            var result = await _userService.GetUserByIdAsync(1);
            Assert.That(result?.Id, Is.EqualTo(1));
        }

        [Test, Category("HappyPath")]
        public async Task GetAllUsersAsync_ShouldReturnList()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { new(), new() });
            var result = await _userService.GetAllUsersAsync();
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test, Category("HappyPath")]
        public async Task UpdateUserAsync_ShouldReturnTrue_OnSuccess()
        {
            var user = new User { Id = 1, Email = "old@test.com" };
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);
            var result = await _userService.UpdateUserAsync(1, new UpdateUserDto { Email = "new@test.com" });
            Assert.That(result, Is.True);
        }

        [Test, Category("HappyPath")]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserDeleted()
        {
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(new User());
            var result = await _userService.DeleteUserAsync(1);
            Assert.That(result, Is.True);
        }

        [Test, Category("HappyPath")]
        public async Task CreateUserAsync_ShouldCallAddAsyncOnce()
        {
            var dto = new CreateUserDto { Email = "unique@test.com", Password = "p", Username="u" };
            await _userService.CreateUserAsync(dto);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Test, Category("HappyPath")]
        public void HashPassword_ShouldGenerateUniqueSalts()
        {
            var pass = "pass";
            var hash1 = ReflectionHash(pass);
            var hash2 = ReflectionHash(pass);
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test, Category("HappyPath")]
        public async Task UpdateUserAsync_ShouldOnlyUpdateProvidedFields()
        {
            var user = new User { Id = 1, Username = "KeepMe", Email = "old@test.com" };
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);
            await _userService.UpdateUserAsync(1, new UpdateUserDto { Email = "new@test.com" });
            Assert.That(user.Username, Is.EqualTo("KeepMe"));
        }

        [Test, Category("HappyPath")]
        public async Task CreateUserAsync_ShouldWorkWithLongPasswords()
        {
            var dto = new CreateUserDto { Email = "long@test.com", Password = new string('a', 100), Username="u" };
            var result = await _userService.CreateUserAsync(dto);
            Assert.That(result, Is.Not.Null);
        }
        #endregion

        #region Edge Case
        [Test, Category("EdgeCase")]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound() =>
            Assert.That(await _userService.GetUserByIdAsync(99), Is.Null);

        [Test, Category("EdgeCase")]
        public async Task UpdateUserAsync_ShouldReturnFalse_WhenUserNotFound() =>
            Assert.That(await _userService.UpdateUserAsync(99, new UpdateUserDto()), Is.False);

        [Test, Category("EdgeCase")]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound() =>
            Assert.That(await _userService.DeleteUserAsync(99), Is.False);

        [Test, Category("EdgeCase")]
        public async Task UpdateUserAsync_ShouldNotCheckEmail_IfEmailIsSame()
        {
            var user = new User { Id = 1, Email = "same@test.com" };
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);
            await _userService.UpdateUserAsync(1, new UpdateUserDto { Email = "same@test.com" });
            _mockRepo.Verify(r => r.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Test, Category("EdgeCase")]
        public async Task CreateUserAsync_ShouldHandleEmojiInUsername()
        {
            var dto = new CreateUserDto { Username = "🚀", Email = "e@e.com", Password = "p" };
            var result = await _userService.CreateUserAsync(dto);
            Assert.That(result.Username, Is.EqualTo("🚀"));
        }

        [Test, Category("EdgeCase")]
        public void VerifyPassword_ShouldReturnFalse_ForWrongPassword()
        {
            var hash = ReflectionHash("CorrectPass");
            Assert.That(_userService.VerifyPassword("WrongPass", hash), Is.False);
        }

        [Test, Category("EdgeCase")]
        public async Task UpdateUserAsync_ShouldWorkWithEmptyDto()
        {
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(new User { Username = "u" });
            Assert.That(await _userService.UpdateUserAsync(1, new UpdateUserDto()), Is.True);
        }

        [Test, Category("EdgeCase")]
        public async Task GetAllUsersAsync_ShouldReturnEmpty_WhenRepoIsEmpty()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            Assert.That(await _userService.GetAllUsersAsync(), Is.Empty);
        }

        [Test, Category("EdgeCase")]
        public void VerifyPassword_ShouldHandleNullPassword() =>
            Assert.Throws<ArgumentNullException>(() => _userService.VerifyPassword(null!, "salt.hash"));
        #endregion

        #region Exception Case
        [Test, Category("ExceptionCase")]
        public void CreateUserAsync_ShouldThrow_WhenEmailExists()
        {
            var dto = new CreateUserDto { Email = "exists@test.com", Password="p", Username="u" };
            _mockRepo.Setup(r => r.FindByEmailAsync(dto.Email)).ReturnsAsync(new User());
            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(dto));
        }

        [Test, Category("ExceptionCase")]
        public void CreateUserAsync_ShouldThrow_WhenUsernameExists()
        {
            var dto = new CreateUserDto { Username = "taken", Email = "exists@test.com", Password = "p" };
            _mockRepo.Setup(r => r.FindByUsernameAsync(dto.Username)).ReturnsAsync(new User());
            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(dto));
        }

        [Test, Category("ExceptionCase")]
        public void UpdateUserAsync_ShouldThrow_WhenNewEmailIsTaken()
        {
            var user = new User { Id = 1, Email = "my@email.com" };
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.FindByEmailAsync("taken@email.com")).ReturnsAsync(new User { Id = 2 });

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.UpdateUserAsync(1, new UpdateUserDto { Email = "taken@email.com" }));
        }

        [Test, Category("ExceptionCase")]
        public void CreateUserAsync_ShouldThrow_IfDtoIsNull() =>
            Assert.ThrowsAsync<NullReferenceException>(() => _userService.CreateUserAsync(null!));

        [Test, Category("ExceptionCase")]
        public void VerifyPassword_ShouldThrow_WhenSaltIsNotBase64() =>
            Assert.Throws<FormatException>(() => _userService.VerifyPassword("p", "!!!.hash"));

        [Test, Category("ExceptionCase")]
        public void UpdateUserAsync_ShouldThrow_IfRepoFails()
        {
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ThrowsAsync(new Exception("DB Down"));
            Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync(1, new UpdateUserDto()));
        }

        [Test, Category("ExceptionCase")]
        public void GetAllUsersAsync_ShouldThrow_OnRepoError()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _userService.GetAllUsersAsync());
        }

        [Test, Category("ExceptionCase")]
        public void CreateUserAsync_ShouldThrow_OnPasswordEncodingError()
        {
            var dto = new CreateUserDto { Password = null!, Email = "exists@test.com", Username = "u" };
            Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateUserAsync(dto));
        }
        #endregion

        // Helper to access private HashPassword via reflection for testing purposes
        private string ReflectionHash(string password)
        {
            var method = typeof(UserService).GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (string)method!.Invoke(_userService, new object[] { password })!;
        }
    }
}
