using Microsoft.AspNetCore.Mvc;
using UsersAPI.Models.DTOs;
using UsersAPI.Models.Entities;
using UsersAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
            Ok(await _userService.GetAllUsersAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            if(id.GetType() != typeof(int)) { return BadRequest("id '" + id + ", is Invalid"); }
            var user = await _userService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto) =>
            await _userService.UpdateUserAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _userService.DeleteUserAsync(id) ? NoContent() : NotFound();
    }

}
