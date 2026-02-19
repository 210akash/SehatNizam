//-----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.GenericRepository
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ERP.Entities.Models;

    /// <summary>
    /// IRepository generic Interface
    /// </summary>
    /// <typeparam name="TEntity">TEntity is class</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Set { get; }
        /// <summary>
        /// Gets Entities
        /// </summary>
        IQueryable<TEntity> Entities
        {
            get;
        }

        /// <summary>
        /// Get the Max Count
        /// </summary>
        /// <param name="filter">the search filter</param>
        /// <returns>the integer</returns>
        long GetCount(Func<TEntity, long> filter);

        /// <summary>
        /// Get the Max Count
        /// </summary>
        /// <param name="filter">the search filter</param>
        /// <returns>the integer</returns>
        Task<long> GetFilterCount(Expression<Func<TEntity, bool>> filter);

        #region GetMethods
        /// <summary>
        /// Get  all IEntity list
        /// </summary>
        /// <returns>IEntity list</returns>
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> FromSqlRaw(string query);

        /// <summary>
        /// Get  all IEntity with cancellation Token
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>IEntity list</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get all with filters
        /// </summary>
        /// <param name="orderBy">The Order by</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <param name="skip">A skip count</param>
        /// <param name="take">A take count</param>
        /// <returns>IEntity list</returns>
        IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        /// <summary>
        /// Async IEntity list with filter
        /// </summary>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <param name="skip">A skip count</param>
        /// <param name="take">A take count</param>
        /// <returns>IEntity list</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        /// <summary>
        /// Get Entity with predict
        /// </summary>
        /// <param name="filter">The filter value</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <param name="skip">A skip count</param>
        /// <param name="take">A take count</param>
        /// <returns>IEntity list</returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        /// <summary>
        /// Get Async Entity with predict
        /// </summary>
        /// <param name="filter">The filter value</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <param name="skip">A skip count</param>
        /// <param name="take">A take count</param>
        /// <returns>The Entity</returns>
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        /// <summary>
        /// Get First Or default entity object
        /// </summary>
        /// <param name="filter">Filter Function</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <returns>Single Entity</returns>
        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = "");

        /// <summary>
        /// Get Async First Or default entity object
        /// </summary>
        /// <param name="filter">Filter Function</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <returns>Single Entity</returns>
        Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null);

        /// <summary>
        /// Get First entity object
        /// </summary>
        /// <param name="filter">Filter Function</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <returns>First Entity</returns>
        TEntity GetFirst(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
           string includeProperties = "");

        /// <summary>
        /// Get Async First entity object
        /// </summary>
        /// <param name="filter">Filter Function</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <returns>First Entity</returns>
        Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null);


        /// <summary>
        /// Get Async First entity object
        /// </summary>
        /// <param name="filter">Filter Function</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <returns>First Entity</returns>
        Task<TEntity> GetFirstAsNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
            string includeProperties = null);


        /// <summary>
        /// Check exists based on prediction
        /// </summary>
        /// <param name="filter">filter Function</param>
        /// <returns>boolean value</returns>
        bool GetExists(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Check Async exists based on prediction
        /// </summary>
        /// <param name="filter">filter Function</param>
        /// <returns>boolean value</returns>
        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null);
        #endregion

        #region Crud Operations
        /// <summary>
        /// Add Object range in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void AddRange(IEnumerable<TEntity> entity);

        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>the task</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entity);

        /// <summary>
        /// Add Object in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>the task</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Update Object in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the range.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void UpdateRange(IEnumerable<TEntity> entity);

        /// <summary>
        /// Detach Entry in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void DetachEntry(TEntity entity);
        void Delete(TEntity entity);

        /// <summary>
        /// Detaches the entry range.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void DetachEntryRange(IEnumerable<TEntity> entity);

        /// <summary>
        /// Modifies the entry.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void ModifyEntry(TEntity entity);

        /// <summary>
        /// Remove object in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void RemoveRange(IEnumerable<TEntity> entity);

        /// <summary>
        /// Save the transection
        /// </summary>
        void Save();

        /// <summary>
        /// Save Async transection
        /// </summary>
        /// <returns>Long value</returns>
        Task<long> SaveAsync();


        /// <summary>
        /// Attach Object in database
        /// </summary>
        /// <param name="entity">Entity object</param>
        void Attach(TEntity entity);

        #endregion

        #region Find
        /// <summary>
        /// Get  entity based on expression
        /// </summary>
        /// <param name="match">The expression</param>
        /// <returns>A Entity</returns>
        TEntity Find(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Get async entity based on expression
        /// </summary>
        /// <param name="match">The expression</param>
        /// <returns>A Entity</returns>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Get entity list based on expression
        /// </summary>
        /// <param name="match">The Expression</param>
        /// <returns>Entity list</returns>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Get  async entity list based on expression
        /// </summary>
        /// <param name="match">The Expression</param>
        /// <returns>Entity list</returns>
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Get  async entity list based on expression
        /// </summary>
        /// <param name="match">The Expression</param>
        /// <returns>Entity list</returns>
        Task<IEnumerable<TEntity>> FindAllIEnumerableAsync(Expression<Func<TEntity, bool>> match);
        #endregion

        /// <summary>
        /// Get Query able Entities with predict
        /// </summary>
        /// <param name="filter">The filter value</param>
        /// <param name="orderBy">The Order By value</param>
        /// <param name="includeProperties">A Include properties</param>
        /// <param name="skip">A skip count</param>
        /// <param name="take">A take count</param>
        /// <returns>The Entity</returns>
        IQueryable<TEntity> GetQueryable(
                    Expression<Func<TEntity, bool>> filter = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDec = null,
                    string includeProperties = null,
                    int? skip = null,
                    int? take = null);

        Tuple<IQueryable<TEntity>, long> GetPagingWhereAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
      PagingData paging = null,
      Expression<Func<TEntity, object>> OrderBy = null,
      Expression<Func<TEntity, object>> OrderByDesc = null,
      List<string> ThenIncludes = null, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Dispose the object
        /// </summary>
        void Dispose();
    }
}
