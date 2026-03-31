namespace UsersAPI.Models.Entities
{
    /// <summary>
    /// Represents the core User domain entity for the application.
    /// This class maps directly to the Users table in the persistent storage.
    /// </summary>
    /// <remarks>
    /// <para><strong>Author:</strong> Joseph Adogeri</para>
    /// <para><strong>Since:</strong> 30-MAR-2026</para>
    /// <para><strong>Version:</strong> 1.0.0</para>
    /// <para><strong>File:</strong> User.cs</para>
    /// </remarks>
    public class User
    {
        /// <summary>
        /// Gets or sets the primary key for the User.
        /// This is an auto-incrementing integer managed by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique username for the account.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique email address for the account.
        /// This field is used for identification and login purposes.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cryptographically secure Argon2id password hash.
        /// <remarks>
        /// This field contains the combined Salt and Hash.
        /// <strong>Warning:</strong> Never expose this field in any API response or DTO.
        /// </remarks>
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
    }
}
