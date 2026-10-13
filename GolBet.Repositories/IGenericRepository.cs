using GolBet.Entities.Common;

namespace GolBet.Repositories
{
    // Restringido a AuditableEntity: toda entidad del dominio ya trae Id + auditoría.
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        Task<T?> GetByIdAsync(int id);

        // Por defecto solo devuelve registros activos: lo desactivado no debe
        // aparecer en listados ni en combos de selección.
        Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false);

        Task AddAsync(T entity);
        void Update(T entity);

        // Baja lógica (IsActive = false). Nunca se borra físicamente: un equipo o
        // partido desactivado puede seguir referenciado por apuestas históricas.
        Task<bool> DeactivateAsync(int id);

        Task<int> SaveChangesAsync();
    }
}
