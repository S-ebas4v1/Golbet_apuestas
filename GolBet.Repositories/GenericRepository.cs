using GolBet.Entities.Common;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AuditableEntity
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(AppDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false)
        {
            IQueryable<T> query = DbSet.AsNoTracking();

            if (!includeInactive)
                query = query.Where(e => e.IsActive);

            return await query.ToListAsync();
        }

        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

        public void Update(T entity) => DbSet.Update(entity);

        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity is null)
                return false;

            // La entidad queda en estado Modified: SaveChangesAsync le estampa ModifiedDate.
            entity.IsActive = false;
            return true;
        }

        public async Task<int> SaveChangesAsync() => await Context.SaveChangesAsync();
    }
}
