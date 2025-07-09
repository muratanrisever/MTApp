using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MTApp.Data
{
    // IDesignTimeDbContextFactory arayüzü, Entity Framework Core tasarım zamanı araçlarına (migrations, scaffolding)
    // ApplicationDbContext'i nasıl oluşturacaklarını öğretir.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // appsettings.json dosyasını bulmak ve yapılandırmayı yüklemek için.
            // Bu, migrations veya scaffolding çalıştırılırken bağlantı dizesini okumak için gereklidir.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Projenin ana dizinini ayarlar
                .AddJsonFile("appsettings.json") // appsettings.json dosyasını ekler
                .Build();

            // Bağlantı dizesini yapılandırmadan alır.
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // DbContextOptionsBuilder kullanarak DbContext için seçenekleri yapılandırır.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString); // SQL Server kullanılacağını belirtir

            // Yapılandırılmış seçeneklerle ApplicationDbContext'in yeni bir örneğini döndürür.
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}