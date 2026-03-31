using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using UsersAPI.Models.DTOs;
using UsersAPI.Models.Entities;
using UsersAPI.Services;

namespace UsersAPI.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private Mock<IUserService> _mockService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockService = new Mock<IUserService>();
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the real service with our Mock
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddScoped(_ => _mockService.Object);
                });
            });
        }

        [SetUp]
        public void SetUp()
        {
            _client = _factory.CreateClient();
            _mockService.Reset(); // Clear mock setups between tests
        }

        [TearDown]
        public void TearDown() => _client?.Dispose();

        [OneTimeTearDown]
        public void OneTimeTearDown() => _factory?.Dispose();

        #region Happy Path
        [Test, Category("HappyPath")]
        public async Task GetAll_Returns200Ok()
        {
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            var response = await _client.GetAsync("/api/users");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, Category("HappyPath")]
        public async Task GetById_Returns200_WhenUserExists()
        {
            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
            var response = await _client.GetAsync("/api/users/1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, Category("HappyPath")]
        public async Task Create_Returns201Created()
        {
            var dto = new CreateUserDto { Username = "u", Email = "e@e.com", Password = "p" };
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(new User { Id = 10 });
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test, Category("HappyPath")]
        public async Task Update_Returns204NoContent()
        {
            _mockService.Setup(s => s.UpdateUserAsync(1, It.IsAny<UpdateUserDto>())).ReturnsAsync(true);
            var response = await _client.PutAsJsonAsync("/api/users/1", new UpdateUserDto());
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, Category("HappyPath")]
        public async Task Delete_Returns204NoContent()
        {
            _mockService.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(true);
            var response = await _client.DeleteAsync("/api/users/1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, Category("HappyPath")]
        public async Task Create_ReturnsCorrectLocationHeader()
        {
            var dto = new CreateUserDto { Username = "u", Email = "e@e.com", Password = "p" };
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(new User { Id = 5 });
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            Assert.That(response.Headers.Location?.ToString(), Does.Contain("/api/Users/5"));
        }

        [Test, Category("HappyPath")]
        public async Task GetById_ReturnsExpectedUserJson()
        {
            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "test" });
            var user = await _client.GetFromJsonAsync<User>("/api/users/1");
            Assert.That(user?.Username, Is.EqualTo("test"));
        }

        [Test, Category("HappyPath")]
        public async Task GetAll_ReturnsEmptyList_WhenNoData()
        {
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            var users = await _client.GetFromJsonAsync<List<User>>("/api/users");
            Assert.That(users, Is.Empty);
        }

        [Test, Category("HappyPath")]
        public async Task Update_PassesCorrectDataToService()
        {
            var dto = new UpdateUserDto { Username = "NewName" };
            _mockService.Setup(s => s.UpdateUserAsync(1, It.IsAny<UpdateUserDto>())).ReturnsAsync(true);
            await _client.PutAsJsonAsync("/api/users/1", dto);
            _mockService.Verify(s => s.UpdateUserAsync(1, It.Is<UpdateUserDto>(d => d.Username == "NewName")), Times.Once);
        }

        [Test, Category("HappyPath")]
        public async Task Create_ReturnsPopulatedUserObject()
        {
            var dto = new CreateUserDto { Username = "u", Email = "e@e.com", Password = "p" };
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(new User { Id = 1, Username = "u" });
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            var user = await response.Content.ReadFromJsonAsync<User>();
            Assert.That(user?.Id, Is.EqualTo(1));
        }
        #endregion

        #region Edge Case
        [Test, Category("EdgeCase")]
        public async Task GetById_Returns404_WhenUserMissing() =>
            Assert.That((await _client.GetAsync("/api/users/99")).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("EdgeCase")]
        public async Task Update_Returns404_WhenUserMissing() =>
            Assert.That((await _client.PutAsJsonAsync("/api/users/99", new UpdateUserDto())).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("EdgeCase")]
        public async Task Delete_Returns404_WhenUserMissing() =>
            Assert.That((await _client.DeleteAsync("/api/users/99")).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("EdgeCase")]
        public async Task GetById_Returns404_WhenIdIsInvalid() =>
            Assert.That((await _client.GetAsync("/api/users/notanint")).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("EdgeCase")]
        public async Task Create_Returns400_WhenRequiredFieldsMissing()
        {
            // Sending empty JSON to trigger validation error on 'required' properties
            var response = await _client.PostAsJsonAsync("/api/users", new { });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Category("EdgeCase")]
        public async Task Update_WorksWithNullProperties()
        {
            _mockService.Setup(s => s.UpdateUserAsync(1, It.IsAny<UpdateUserDto>())).ReturnsAsync(true);
            var response = await _client.PutAsJsonAsync("/api/users/1", new UpdateUserDto { Username = null });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, Category("EdgeCase")]
        public async Task Delete_WorksWithLargeId() =>
            Assert.That((await _client.DeleteAsync($"/api/users/{int.MaxValue}")).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("EdgeCase")]
        public async Task Create_HandlesEmojiInput()
        {
            var dto = new CreateUserDto { Username = "🔥", Email = "e@e.com", Password = "p" };
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(new User());
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test, Category("EdgeCase")]
        public async Task Post_Returns415_ForWrongContentType()
        {
            var response = await _client.PostAsync("/api/users", new StringContent("text", System.Text.Encoding.UTF8, "text/plain"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
        }

       #endregion

        #region Exception Case

        [Test, Category("ExceptionCase")]
        public async Task GetAll_Returns500_OnServiceCrash()
        {
            _mockService.Setup(s => s.GetAllUsersAsync()).ThrowsAsync(new Exception());
            var response = await _client.GetAsync("/api/users");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }


        [Test, Category("ExceptionCase")]
        public async Task Post_Returns400_OnMalformedJson()
        {
            var response = await _client.PostAsync("/api/users", new StringContent("{", System.Text.Encoding.UTF8, "application/json"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Category("ExceptionCase")]
        public async Task GetById_Returns404_OnNegativeId() =>
            Assert.That((await _client.GetAsync("/api/users/-5")).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        [Test, Category("ExceptionCase")]
        public async Task Delete_Returns500_OnDatabaseTimeout()
        {
            _mockService.Setup(s => s.DeleteUserAsync(1)).ThrowsAsync(new TaskCanceledException());
            var response = await _client.DeleteAsync("/api/users/1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test, Category("ExceptionCase")]
        public async Task Request_Returns405_ForUnsupportedPatch() =>
            Assert.That((await _client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, "/api/users/1"))).StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));

        [Test, Category("ExceptionCase")]
        public async Task Create_Fails_WhenServiceReturnsNull()
        {
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync((User)null!);
            var dto = new CreateUserDto { Username = "u", Email = "e", Password = "p" };
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            Assert.That((int)response.StatusCode, Is.AtLeast(400));
        }

        [Test, Category("ExceptionCase")]
        public async Task Post_ReturnsInternalError_WhenServiceThrowsGenericException()
        {
            _mockService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>())).ThrowsAsync(new Exception("Generic Error"));
            var dto = new CreateUserDto { Username = "u", Email = "e", Password = "p" };
            var response = await _client.PostAsJsonAsync("/api/users", dto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }
        #endregion
    }
}
