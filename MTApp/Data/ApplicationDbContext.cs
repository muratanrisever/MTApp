using Microsoft.EntityFrameworkCore;
using MTApp.Models; // Kendi modelleriniz (User, Employee, Department vb.) için

namespace MTApp.Data
{
    // Artık IdentityDbContext yerine DbContext'ten kalıtım alıyoruz.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Kullanıcı modelimizi DbSet olarak tanımlıyoruz.
        public DbSet<User> Users { get; set; } = default!;

        // Diğer modelleriniz...
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
            // Model oluşturma sırasında ek konfigürasyonlar veya kısıtlamalar ekleyebilirsiniz.
            // Örneğin, Kullanıcı Adı'nın benzersiz olmasını sağlamak:
            builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}