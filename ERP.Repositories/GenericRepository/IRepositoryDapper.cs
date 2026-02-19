//-----------------------------------------------------------------------
// <copyright file="IRepositoryDapper.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.GenericRepository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository Dapper
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepositoryDapper<TEntity> where TEntity : class
    {
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        T ExecuteScalar<T>(string sql, object param);

        /// <summary>
        /// Queries the first or default.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        T QueryFirstOrDefault<T>(string sql, object param);

        /// <summary>
        /// Queries the first or default asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param);

        /// <summary>
        /// Queries the single or default.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        T QuerySingleOrDefault<T>(string sql, object param);

        /// <summary>
        /// Queries the single or default asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param);

        /// <summary>
        /// Queries the multiple.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        HashSet<T> QueryMultiple<T>(string sql, object param);

        /// <summary>
        /// Queries the multiple asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        Task<HashSet<T>> QueryMultipleAsync<T>(string sql, object param);

        /// <summary>
        /// Queries the multiple integer.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        HashSet<int> QueryMultipleInt<T>(string sql, object param);

        /// <summary>
        /// Queries the multiple integer asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        Task<HashSet<int>> QueryMultipleIntAsync<T>(string sql, object param);

        /// <summary>
        /// Queries the specified SQL.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        IEnumerable<T> Query<T>(string sql, object param = null);

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>the entity</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        void Execute(string sql, object param);

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        void ExecuteAsync(string sql, object param);

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>the long count</returns>
        long Insert(TEntity obj, int timeout = 1500);

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>the long count</returns>
        Task<long> InsertAsync(TEntity obj, int timeout = 1500);

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>return true or false</returns>
        bool Update(TEntity obj, int timeout = 1500);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>return true or false</returns>
        Task<bool> UpdateAsync(TEntity obj, int timeout = 1500);
    }
}
