//-----------------------------------------------------------------------
// <copyright file="IUnitOfWorkDapper.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Repositories.UnitOfWork
{
    using System;
    using ERP.Repositories.GenericRepository;

    /// <summary>
    /// Unit Of Work Dapper
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWorkDapper : IDisposable
    {
        /// <summary>
        /// Commits this instance.
        /// </summary>
        void Commit();

        /// <summary>
        /// Repositories this instance.
        /// </summary>
        /// <typeparam name="T">the class</typeparam>
        /// <returns>The Dapper Repo</returns>
        IRepositoryDapper<T> Repository<T>() where T : class;
    }
}
