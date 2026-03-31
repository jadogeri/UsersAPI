using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.Entities;

namespace UsersAPI.Data
{
    /// <summary>
    /// The primary Entity Framework Core database context for the UsersAPI.
    /// Handles the mapping between C# domain models and the underlying database schema.
    /// </summary>
    /// <remarks>
    /// <para><strong>Author:</strong> Joseph Adogeri</para>
    /// <para><strong>Since:</strong> 30-MAR-2026</para>
    /// </remarks>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="DbContext"/> (e.g., Connection String, SQL Provider).</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Users table.
        /// Represents the persistent storage for user account records.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Configures the database model during the initialization process.
        /// Used to define constraints, indexes, and seed data.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the User entity
            modelBuilder.Entity<User>(entity =>
            {
                // Ensure the Email column has a Unique Index to prevent duplicate registrations
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // Optional: Ensure the Username is also unique if required by business logic
                entity.HasIndex(u => u.Username)
                      .IsUnique();
            });
        }
    }
}
