using Microsoft.EntityFrameworkCore;
using TeknikServis.Application.Interfaces;
using TeknikServis.Infrastructure.Persistence;

namespace TeknikServis.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }

        // YENİ EKLENEN METOT
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // AsNoTracking: "Bu veriyi sadece okuyacağım, üzerinde değişiklik yapmayacağım" demektir. Performansı çok artırır.
            return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            // Not: AppDbContext'teki ayarımız sayesinde Remove desek bile veriyi gerçekten silmez, sadece IsDeleted = true yapar (Soft Delete).
        }

        public async Task<IReadOnlyList<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // AsNoTracking ile performansı artırarak şarta uyanları listeliyoruz
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
    }
}