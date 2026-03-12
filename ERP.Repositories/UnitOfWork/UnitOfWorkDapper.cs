//-----------------------------------------------------------------------
// <copyright file="UnitOfWorkDapper.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.UnitOfWork
{
    using System;
    using System.Collections;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using ERP.Repositories.GenericRepository;

    /// <summary>
    /// Unit Of Work Dapper
    /// </summary>
    /// <seealso cref="ERP.Repositories.UnitOfWork.IUnitOfWorkDapper" />
    public class UnitOfWorkDapper : IUnitOfWorkDapper
    {
        #region Fields

        /// <summary>
        /// The connection
        /// </summary>
        private IDbConnection connection;

        /// <summary>
        /// The transaction
        /// </summary>
        private IDbTransaction transaction;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The repositories
        /// </summary>
        private Hashtable repositories;

        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string connectionString;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkDapper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public UnitOfWorkDapper(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            this.connection.Open();
            this.transaction = this.connection.BeginTransaction();
            this.connectionString = connectionString;
        }

        #region IUnitOfWorkDapper Members

        /// <summary>
        /// Finalizes an instance of the <see cref="UnitOfWorkDapper"/> class.
        /// </summary>
        ~UnitOfWorkDapper()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit()
        {
            try
            {
                this.transaction.Commit();
            }
            catch
            {
                this.transaction.Rollback();
            }
            finally
            {
                this.transaction.Dispose();
                if (this.connection.State == ConnectionState.Closed)
                {

                    this.connection = new SqlConnection(this.connectionString);
                    this.connection.Open();
                }
                this.repositories = null;
                this.transaction = this.connection.BeginTransaction();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Repositories this instance.
        /// </summary>
        /// <typeparam name="T">The class</typeparam>
        /// <returns>
        /// The Dapper Repo
        /// </returns>
        public IRepositoryDapper<T> Repository<T>() where T : class
        {
            if (this.repositories == null)
            {
                this.repositories = new Hashtable();
            }

            var type = typeof(T).Name;

            if (!this.repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositoryDapper<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), this.transaction, this.connection);
                this.repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryDapper<T>)this.repositories[type];
        }

        #region Private Methods

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.transaction != null)
                    {
                        this.transaction.Dispose();
                        this.transaction = null;
                    }

                    if (this.connection != null)
                    {
                        this.connection.Dispose();
                        this.connection = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
