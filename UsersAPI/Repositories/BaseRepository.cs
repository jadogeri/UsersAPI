using Microsoft.EntityFrameworkCore;


namespace UsersAPI.Repositories
{
    public class BaseRepository<T, TContext> where T : class where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // READ ALL
        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        // READ ONE
        public virtual async Task<T?> FindByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        // CREATE
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); // In C#, you must call SaveChanges to persist
        }

        // UPDATE
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // DELETE
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await FindByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }

}
