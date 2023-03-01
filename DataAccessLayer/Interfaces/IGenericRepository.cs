using System.Linq.Expressions;

namespace DataAccessLayer.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity?> FindAsync(params object[] predicate);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        Task<List<TEntity>> GetAllAsync();
        void RemoveRange(List<TEntity> list);
        void Remove(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FirstOrDefaultAsync();
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CommitAsync();    
    }
}
