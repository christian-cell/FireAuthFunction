using System.Linq.Expressions;
using FireAuth.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FireAuth.Repository
{
    public class EntityRepository : IEntityRepository
    {
    protected readonly DbContext _context;

        public EntityRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        public async Task<TEntityBase?> FindAsync<TEntityBase>(params object[] keys) where TEntityBase : class, IEntityBase
        {
            return await _context.Set<TEntityBase>().FindAsync(keys).ConfigureAwait(false);
        }

        public async Task<IQueryable<TEntityBase>> SearchAsync<TEntityBase>(int currentPage, int pageSize, Expression<Func<TEntityBase, bool>>? filter) where TEntityBase : class, IEntityBase
        {
            if (filter == null)
            {
                return await Task.FromResult(_context.Set<TEntityBase>()
                    .Where(x => !x.Deleted)
                    .Skip(currentPage)
                    .Take(pageSize)
                    .AsQueryable())
                    .ConfigureAwait(false);
            }
            else
            {
                return await Task.FromResult(_context.Set<TEntityBase>()
                    .Where(x => !x.Deleted)
                    .Where(filter)
                    .Skip(currentPage)
                    .Take(pageSize))
                    .ConfigureAwait(false);
            }
        }

        public async Task<IQueryable<TEntityBase>> SearchAsync<TEntityBase>(Expression<Func<TEntityBase, bool>>? filter) where TEntityBase : class, IEntityBase
        {
            if (filter == null)
            {
                return await Task.FromResult(_context.Set<TEntityBase>()
                    .Where(x => !x.Deleted)
                    .AsQueryable())
                    .ConfigureAwait(false);
            }
            else
            {
                return await Task.FromResult(_context.Set<TEntityBase>()
                    .Where(x => !x.Deleted)
                    .Where(filter))
                    .ConfigureAwait(false);
            }
        }

        public async Task<TEntityBase> AddAsync<TEntityBase>(TEntityBase entity, string userName = "System") where TEntityBase : class, IEntityBase
        {
            entity.CreatedOn = DateTime.Now;
            entity.CreatedByUser = userName;

            await _context.Set<TEntityBase>().AddAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public async Task<bool> UpdateAsync<TEntityBase>(Guid entityId, TEntityBase entity, string userName = "System") where TEntityBase : class, IEntityBase
        {
            var entityDb = await _context.Set<TEntityBase>().FindAsync(entityId).ConfigureAwait(false);

            if (entityDb is null)
                return false;

            entity.ModifiedOn = DateTime.Now;
            entity.ModifiedByUser = userName;
            entity.CreatedByUser = entityDb.CreatedByUser;
            entity.CreatedOn = entityDb.CreatedOn;
            entity.DeletedByUser = entityDb.DeletedByUser;
            entity.DeletedOn = entityDb.DeletedOn;

            _context.Entry(entityDb).CurrentValues.SetValues(entity);

            return true;
        }

        public async Task<bool> DeleteAsync<TEntityBase>(Guid entityId, string userName = "System") where TEntityBase : class, IEntityBase
        {
            var entity = await _context.Set<TEntityBase>().FindAsync(entityId).ConfigureAwait(false);

            if (entity is null || entity.Deleted)
                return false;

            entity.Deleted = true;
            entity.DeletedOn = DateTime.Now;
            entity.DeletedByUser = userName;

            return true;
        }

        public async Task<bool> DeleteHardAsync<TEntityBase>(Guid entityId) where TEntityBase : class, IEntityBase
        {
            var entity = await _context.Set<TEntityBase>().FindAsync(entityId).ConfigureAwait(false);

            if (entity is null)
                return false;

            _context.Remove(entity);

            return true;
        }

        public async Task<bool> ExistsAsync<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase
        {
            return await _context.Set<TEntityBase>().AnyAsync(filter).ConfigureAwait(false);
        }

        public bool Exists<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase
        {
            return _context.Set<TEntityBase>().Any(filter);
        }

        public async Task<TEntityBase?> FirstOrDefaultAsync<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase
        {
            return await _context.Set<TEntityBase>().FirstOrDefaultAsync(filter).ConfigureAwait(false);
        }
    }
};

