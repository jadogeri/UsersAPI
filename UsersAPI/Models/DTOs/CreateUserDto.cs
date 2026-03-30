namespace UsersAPI.Models.DTOs
{
    // DTO for Creating a User (No ID required)
    public class CreateUserDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }


}