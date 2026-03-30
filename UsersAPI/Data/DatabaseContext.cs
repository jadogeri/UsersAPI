using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.Entities;

namespace UsersAPI.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Seed initial data or configure constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}