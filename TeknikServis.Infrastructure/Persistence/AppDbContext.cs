using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TeknikServis.Domain.Common;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Entities.Devices;
using TeknikServis.Domain.Entities.WorkOrders;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Infrastructure.Persistence
{
    // DbContext yerine IdentityDbContext kullanıyoruz (User/Role/Key)
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanındaki tablolarımız
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity tablolarının (AspNetUsers vs.) oluşması için ŞART!
            base.OnModelCreating(modelBuilder);

            // Configurations klasöründeki tüm ayarları otomatik bul ve uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        // Her kaydetme işleminden önce araya girip tarihleri otomatik atıyoruz
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}