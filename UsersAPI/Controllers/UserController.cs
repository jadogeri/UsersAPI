using Microsoft.AspNetCore.Mvc;
using UsersAPI.Models.DTOs;
using UsersAPI.Models.Entities;
using UsersAPI.Services;

namespace UsersAPI.Controllers
{
    /// <summary>
    /// RESTful API Controller for User Management.
    /// Handles HTTP requests for User CRUD operations.
    /// </summary>
    /// <remarks>
    /// <para><strong>Author:</strong> Joseph Adogeri</para>
    /// <para><strong>Since:</strong> 30-MAR-2026</para>
    /// <para><strong>Version:</strong> 1.0.0</para>
    /// <para><strong>File:</strong> UsersController.cs</para>
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>A list of User entities.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
            Ok(await _userService.GetAllUsersAsync());

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique integer ID of the user.</param>
        /// <returns>The requested User entity if found; otherwise, NotFound.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetById(int id)
        {
            // Note: The :int route constraint ensures 'id' is a valid integer before execution.
            var user = await _userService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        /// <summary>
        /// Creates and registers a new user in the system.
        /// </summary>
        /// <param name="dto">The data transfer object containing username, email, and password.</param>
        /// <returns>The newly created user with their generated ID.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> Create(CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Updates the profile information of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="dto">The data to update (Username or Email).</param>
        /// <returns>NoContent if successful; NotFound if the user does not exist.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto) =>
            await _userService.UpdateUserAsync(id, dto) ? NoContent() : NotFound();

        /// <summary>
        /// Permanently removes a user from the system.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>NoContent if successful; NotFound if the user does not exist.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) =>
            await _userService.DeleteUserAsync(id) ? NoContent() : NotFound();
    }
}
