namespace ERP.Repositories.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using ERP.Entities.Models;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ERPDbContext context;
        private DbSet<TEntity> set;
        private bool disposed = false;

        public Repository(ERPDbContext context)
        {
            this.context = context;
        }

        public virtual IQueryable<TEntity> Entities
        {
            get { return this.Set; }
        }

        public DbSet<TEntity> Set
        {
            get { return this.set ?? (this.set = this.context.Set<TEntity>()); }
        }

        #region GetMethods
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await this.Set?.ToListAsync();
        }

        public virtual async Task<List<TEntity>> FromSqlRaw(string query)
        {
            return await this.Set?.FromSqlRaw(query).AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await this.Set.ToListAsync(cancellationToken);
        }

        public virtual IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
        {
            return this.GetQueryable(null, orderBy, orderByDec, includeProperties, skip, take);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
        {
            return await this.GetQueryable(null, orderBy, orderByDec, includeProperties, skip, take).ToListAsync();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
        {
            return this.GetQueryable(filter, orderBy, orderByDec, includeProperties, skip, take);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
        {
            return await this.GetQueryable(filter, orderBy, orderByDec, includeProperties, skip, take).ToListAsync();
        }

        public virtual TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = "")
        {
            return this.GetQueryable(filter, null, orderByDec, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null)
        {
            return await this.GetQueryable(filter, null, orderByDec, includeProperties).FirstOrDefaultAsync();
        }

        public virtual TEntity GetFirst(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
           string includeProperties = "")
        {
            return this.GetQueryable(filter, orderBy, orderByDec, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null)
        {
            return await this.GetQueryable(filter, orderBy, orderByDec, includeProperties).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> GetFirstAsNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null)
        {
            return await this.GetQueryable(filter, orderBy, orderByDec, includeProperties).AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual long GetCount(Func<TEntity, long> filter)
        {
            if (this.Set.Count() <= 0)
            {
                return 0;
            }
            else
            {
                return this.Set.Max(filter);
            }
        }

        public virtual async Task<long> GetFilterCount(Expression<Func<TEntity, bool>> filter)
        {
                return (long)await this.GetQueryable(filter).CountAsync();
        }

        public virtual bool GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            return this.GetQueryable(filter).Any();
        }

        public virtual Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return this.GetQueryable(filter).AnyAsync();
        }
        #endregion

        #region Crud Operations
        public virtual void Add(TEntity entity)
        {
            this.Set.Add(entity);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await this.Set.AddAsync(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entity)
        {
            this.Set.AddRange(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entity)
        {
            await this.Set.AddRangeAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            this.Set.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entity)
        {
            this.Set.UpdateRange(entity);
        }

        public virtual void DetachEntry(TEntity entity)
        {
            this.context.Entry(entity).State = EntityState.Detached;
        }
        public virtual void Delete(TEntity entity)
        {
            this.context.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void DetachEntryRange(IEnumerable<TEntity> entity)
        {
            this.context.Entry<IEnumerable<TEntity>>(entity).State = EntityState.Detached;
        }

        public virtual void ModifyEntry(TEntity entity)
        {
            this.context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(TEntity entity)
        {
            this.Set.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entity)
        {
            this.Set.RemoveRange(entity);
        }

        public virtual void Save()
        {
            this.context.SaveChanges();
        }

        public async virtual Task<long> SaveAsync()
        {
            return await this.context.SaveChangesAsync();
        }

        public virtual void Attach(TEntity entity)
        {
            this.context.Attach(entity);
        }

        #endregion



        #region Find
        public virtual TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return this.Set.FirstOrDefault(match);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.Set.FirstOrDefaultAsync(match);
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return this.Set.Where(match);
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.Set.Where(match).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAllIEnumerableAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.Set.Where(match).ToListAsync();
        }

        #endregion

        public virtual IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderByDesc = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = this.Set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (OrderByDesc != null)
            {
                query = OrderByDesc(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        public Tuple<IQueryable<TEntity>, long> GetPagingWhereAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
         PagingData paging = null,
         Expression<Func<TEntity, object>> OrderBy = null,
         Expression<Func<TEntity, object>> OrderByDesc = null,
         List<string> ThenIncludes = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = this.Set;
            var result = query.AsNoTracking().Where(predicate);
            if (includes != null) foreach (var includeExpression in includes) result = result.Include(includeExpression);
            if (ThenIncludes != null) foreach (var includeExpression in ThenIncludes) result = result.Include(includeExpression);

            if (OrderBy != null) result = result.OrderBy(OrderBy);
            if (OrderByDesc != null) result = result.OrderByDescending(OrderByDesc);

            var tempresult = result;

            if (paging != null) if (paging.IsPagingEnabled) result = result.Skip(paging.Skip).Take(paging.Take);

            return new Tuple<IQueryable<TEntity>, long>(result, tempresult.Count());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
