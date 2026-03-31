namespace UsersAPI.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for updating an existing User's profile information.
    /// Supports partial updates (Patch-like behavior) by allowing properties to be null.
    /// </summary>
    /// <remarks>
    /// <para><strong>Author:</strong> Joseph Adogeri</para>
    /// <para><strong>Since:</strong> 30-MAR-2026</para>
    /// <para><strong>File:</strong> UpdateUserDto.cs</para>
    /// </remarks>
    public class UpdateUserDto
    {
        /// <summary>
        /// Gets or sets the new username. 
        /// If null, the existing username remains unchanged.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the new email address. 
        /// If provided, it must be unique within the system.
        /// </summary>
        public string? Email { get; set; }
    }
}
