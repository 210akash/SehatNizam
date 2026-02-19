//-----------------------------------------------------------------------
// <copyright file="RepositoryDapper.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;

    /// <summary>
    /// Repository Dapper
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="ERP.Repositories.GenericRepository.IRepositoryDapper{TEntity}" />
    public class RepositoryDapper<TEntity> : IRepositoryDapper<TEntity> where TEntity : class
    {
        /// <summary>
        /// The transaction
        /// </summary>
        private IDbTransaction transaction;

        /// <summary>
        /// The connection
        /// </summary>
        private IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDapper{TEntity}"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="databaseConnection">The database connection.</param>
        public RepositoryDapper(IDbTransaction transaction, IDbConnection databaseConnection)
        {
            this.transaction = transaction;
            this.connection = databaseConnection;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public T ExecuteScalar<T>(string sql, object param)
        {
            return this.connection.ExecuteScalar<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the single or default.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public T QuerySingleOrDefault<T>(string sql, object param)
        {
            return this.connection.QuerySingleOrDefault<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the single or default asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param)
        {
            return await this.connection.QuerySingleOrDefaultAsync<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the multiple.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public HashSet<T> QueryMultiple<T>(string sql, object param)
        {
            try
            {
                var result = this.connection.QueryMultiple(sql, param: param, transaction: this.transaction, commandType: CommandType.StoredProcedure);
                var data = result.Read<T>().ToHashSet();
                return data;
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();
            }

            return new HashSet<T>();
        }

        /// <summary>
        /// Queries the multiple asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public async Task<HashSet<T>> QueryMultipleAsync<T>(string sql, object param)
        {
            try
            {
                var result = await this.connection.QueryMultipleAsync(sql, param: param, transaction: this.transaction, commandType: CommandType.StoredProcedure);
                var data = result.ReadAsync<T>().Result.ToHashSet();
                return data;
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();
            }

            return new HashSet<T>();
        }

        /// <summary>
        /// Queries the multiple integer.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public HashSet<int> QueryMultipleInt<T>(string sql, object param)
        {
            try
            {
                var result = this.connection.QueryMultiple(sql, param: param, transaction: this.transaction, commandType: CommandType.StoredProcedure);
                var data = result.Read<int>().ToHashSet();
                return data;
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();
            }

            return new HashSet<int>();
        }

        /// <summary>
        /// Queries the multiple integer asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public async Task<HashSet<int>> QueryMultipleIntAsync<T>(string sql, object param)
        {
            try
            {
                var result = await this.connection.QueryMultipleAsync(sql, param: param, transaction: this.transaction, commandType: CommandType.StoredProcedure);
                var data = result.ReadAsync<int>().Result.ToHashSet();
                return data;
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();
            }

            return new HashSet<int>();
        }

        /// <summary>
        /// Queries the first or default.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public T QueryFirstOrDefault<T>(string sql, object param)
        {
            return this.connection.QueryFirstOrDefault<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the first or default asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param)
        {
            return await this.connection.QueryFirstOrDefaultAsync<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the specified SQL.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return this.connection.Query<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// the entity
        /// </returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return await this.connection.QueryAsync<T>(sql, param, this.transaction);
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        public void Execute(string sql, object param)
        {
            this.connection.Execute(sql, param, this.transaction);
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The parameter.</param>
        public void ExecuteAsync(string sql, object param)
        {
            this.connection.ExecuteAsync(sql, param, this.transaction);
        }

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>the long count</returns>
        public long Insert(TEntity obj, int timeout = 1500)
        {
            return this.connection.Insert<TEntity>(obj, this.transaction, timeout);
        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>the long count</returns>
        public async Task<long> InsertAsync(TEntity obj, int timeout = 1500)
        {
            return await this.connection.InsertAsync<TEntity>(obj, this.transaction, timeout);
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>return true or false</returns>
        public bool Update(TEntity obj, int timeout = 1500)
        {
            return this.connection.Update<TEntity>(obj, this.transaction, timeout);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>return true or false</returns>
        public async Task<bool> UpdateAsync(TEntity obj, int timeout = 1500)
        {
            return await this.connection.UpdateAsync<TEntity>(obj, this.transaction, timeout);
        }
    }
}
