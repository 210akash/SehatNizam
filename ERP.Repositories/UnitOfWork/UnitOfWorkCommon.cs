//-----------------------------------------------------------------------
// <copyright file="UnitOfWorkCommon.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace Possession.Repositories.UnitOfWorkCommon
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Possession.Entities.Models;
    using Possession.Repositories.GenericRepository;
    using Possession.Repositories.UnitOfWork;

    /// <summary>
    /// The Unit of work class
    /// </summary>
    public class UnitOfWorkCommon : IUnitOfWorkCommon
    {
        #region Fields
        /// <summary>
        /// Context declare
        /// </summary>
        private readonly SensyrtechContextCommon context;

        /// <summary>
        /// Repositories declare
        /// </summary>
        private Hashtable repositories;
        #endregion

        #region Constructors                
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCommon"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWorkCommon(SensyrtechContextCommon context)
        {
            this.context = context;
        }

        #endregion
        // todo : currently this has 55 refrences we should really be using 
        //       this function's async sibling instead.

        /// <summary>
        /// Save Transaction
        /// </summary>
        /// <returns>Response integer</returns>
        public int SaveChanges()
        {
            try
            {
                return this.context.SaveChanges();
            }
            finally
            {
                this.repositories = null;
            }
        }

        /// <summary>
        /// Save transection async
        /// </summary>
        /// <returns>Response value</returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await this.context.SaveChangesAsync();
            }
            finally
            {
                this.repositories = null;
            }
        }

        [Obsolete]
        public async Task<int> ExecuteSqlCommandAsync(string query)
        {
            try
            {
                return await this.context.Database.ExecuteSqlRawAsync(query);
            }
            finally
            {
                this.repositories = null;
            }
        }
        [Obsolete]
        public DatabaseFacade Database()
        {
            try
            {
                return  this.context.Database;
            }
            finally
            {
                this.repositories = null;
            }
        }

        /// <summary>
        /// Save transection async
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Response number</returns>
        public async Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                return await this.context.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                this.repositories = null;
            }
        }

        /// <summary>
        /// Register the repository
        /// </summary>
        /// <typeparam name="T">T is the class</typeparam>
        /// <returns>A repository</returns>
        public IRepository<T> Repository<T>() where T : class
        {
            if (this.repositories == null)
            {
                this.repositories = new Hashtable();
            }

            var type = typeof(T).Name;

            if (!this.repositories.ContainsKey(type))
            {
               Type repositoryType = typeof(Possession.Repositories.GenericRepository.RepositoryCommon<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), this.context);
                this.repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)this.repositories[type];
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose context
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
        }
        #endregion
    }
}
