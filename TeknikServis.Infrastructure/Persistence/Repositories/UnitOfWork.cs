using TeknikServis.Application.Interfaces;
using TeknikServis.Infrastructure.Persistence;

namespace TeknikServis.Infrastructure.Persistence.Repositories
{
    // IUnitOfWork sözleşmesini imzalıyoruz
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // AppDbContext içindeki SaveChangesAsync metodunu tetikler
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}