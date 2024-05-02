using System.Linq.Expressions;
using FireAuth.Repository.Entities;

namespace FireAuth.Repository
{
    public interface IEntityRepository
    {
        Task<bool> SaveAsync();
        Task<TEntityBase?> FindAsync<TEntityBase>(params object[] keys) where TEntityBase : class, IEntityBase;
        Task<IQueryable<TEntityBase>> SearchAsync<TEntityBase>(Expression<Func<TEntityBase, bool>>? filter = null) where TEntityBase : class, IEntityBase;
        Task<IQueryable<TEntityBase>> SearchAsync<TEntityBase>(int currentPage, int pageSize, Expression<Func<TEntityBase, bool>>? filter = null) where TEntityBase : class, IEntityBase;
        Task<TEntityBase> AddAsync<TEntityBase>(TEntityBase entity, string userName = "System") where TEntityBase : class, IEntityBase;
        Task<bool> UpdateAsync<TEntityBase>(Guid entityId, TEntityBase entity, string userName = "System") where TEntityBase : class, IEntityBase;
        Task<bool> DeleteAsync<TEntityBase>(Guid entityId, string userName = "System") where TEntityBase : class, IEntityBase;
        Task<bool> DeleteHardAsync<TEntityBase>(Guid entityId) where TEntityBase : class, IEntityBase;
        Task<bool> ExistsAsync<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase;
        bool Exists<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase;
        Task<TEntityBase?> FirstOrDefaultAsync<TEntityBase>(Expression<Func<TEntityBase, bool>> filter) where TEntityBase : class, IEntityBase;
    }
};

