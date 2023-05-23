using IrzUccApi.Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IrzUccApi.Db.Repositories
{
    public abstract class AppRepository<TEntity, TDbContext>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;

        public AppRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(TEntity entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
            => _dbContext.Set<TEntity>().Remove(entity);

        public async Task<TEntity?> GetByIdAsync(Guid id)
            => await _dbContext.Set<TEntity>().FindAsync(id);

        public void Update(TEntity entity)
            => _dbContext.Entry(entity).State = EntityState.Modified;

        public async Task<bool> ExistsAsync(Guid id)
            => await ExistsAsync(e => e.Id == id);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbContext.Set<TEntity>().AnyAsync(predicate);
    }
}