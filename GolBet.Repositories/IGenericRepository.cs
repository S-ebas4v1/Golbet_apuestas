using GolBet.Entities.Common;

namespace GolBet.Repositories
{
    // Restringido a AuditableEntity: toda entidad del dominio ya trae Id + auditoría.
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
