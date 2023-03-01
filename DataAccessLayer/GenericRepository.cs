using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace DataAccessLayer
{
    public class GenericRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DbContext dbContext;
        private DbSet<TEntity> DbSet
        {
            get
            {
                return dbContext.Set<TEntity>();
            }
        }
        public GenericRepository(DbContext DbContext)
        {
            dbContext = DbContext;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await DbSet.AddAsync(entity)).Entity;
        }
        public async Task<TEntity?> FindAsync(params object[] predicate)
        {
            return await DbSet.FindAsync(predicate);
        }
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.FirstAsync(predicate);
        }
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }
        public async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }
        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }
        public void RemoveRange(List<TEntity> list)
        {
            foreach (var item in list)
                Remove(item);
        }
        public void Remove(TEntity enity)
        {
            DbSet.Remove(enity);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }
        public async Task<TEntity?> FirstOrDefaultAsync()
        {
            return await DbSet.FirstOrDefaultAsync();
        }
        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync(predicate);
        }     
        public async Task<int> CommitAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
