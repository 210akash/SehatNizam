//-----------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.UnitOfWork
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using ERP.Repositories.GenericRepository;

    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Register the repository
        /// </summary>
        /// <typeparam name="TEntity">TEntity is the class</typeparam>
        /// <returns>A repository</returns>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        /// <summary>
        /// Save transaction 
        /// </summary>
        /// <returns>Response integer</returns>
        int SaveChanges();

        /// <summary>
        /// Save transaction async
        /// </summary>
        /// <returns>Response integer</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Save transaction async with cancellation token
        /// </summary>
        /// <param name="cancellationToken">A cancellation Token</param>
        /// <returns>Response integer</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> ExecuteSqlCommandAsync(string query);
        DatabaseFacade Database();
    }
}
