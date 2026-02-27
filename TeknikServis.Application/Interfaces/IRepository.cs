using System.Linq.Expressions;

namespace TeknikServis.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        // YENİ EKLENEN SATIR: Tüm verileri liste halinde okumak için
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);


        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}