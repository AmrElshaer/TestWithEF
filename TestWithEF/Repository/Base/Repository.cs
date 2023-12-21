using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TestWithEF.Base;
using TestWithEF.Entities;
using TestWithEF.IRepositories.Base;

namespace TestWithEF.Repository.Base
{
    public class Repository<T, TId> : IRepository<T, TId>
        where T : Entity<TId>
        where TId : IComparable<TId>
    {
        protected readonly TestDbContext DbDbContext;

        public Repository(TestDbContext dbDbContext)
        {
            DbDbContext = dbDbContext ?? throw new ArgumentNullException(nameof(dbDbContext));
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await DbDbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T, TId>.GetQuery(DbDbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbDbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync
            (Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = DbDbContext.Set<T>();

            if (disableTracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync
        (
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true
        )
        {
            IQueryable<T> query = DbDbContext.Set<T>();

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(TId id)
        {
            return await DbDbContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await DbDbContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            DbDbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            DbDbContext.Set<T>().Remove(entity);
        }
    }
}
