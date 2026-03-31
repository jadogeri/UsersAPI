namespace UsersAPI.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for creating a new User record.
    /// This model encapsulates the mandatory data required for initial registration.
    /// </summary>
    /// <remarks>
    /// <para><strong>Author:</strong> Joseph Adogeri</para>
    /// <para><strong>Since:</strong> 30-MAR-2026</para>
    /// <para><strong>File:</strong> CreateUserDto.cs</para>
    /// </remarks>
    public class CreateUserDto
    {
        /// <summary>
        /// Gets or sets the desired unique handle for the user.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address to be used for account identification and communication.
        /// This field is enforced as unique in the database schema.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the raw password string.
        /// This is never stored directly but is processed via Argon2id hashing before persistence.
        /// </summary>
        public required string Password { get; set; }
    }
}
