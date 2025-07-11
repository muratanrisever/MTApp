using Microsoft.EntityFrameworkCore;
using MTApp.Models;

namespace MTApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;

        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;
        public DbSet<Title> Titles { get; set; } = default!;
        public DbSet<Leave> Leaves { get; set; } = default!;
        public DbSet<Advance> Advances { get; set; } = default!;
        public DbSet<Training> Trainings { get; set; } = default!;
        public DbSet<Announcement> Announcements { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}