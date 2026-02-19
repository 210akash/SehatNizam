namespace ERP.Repositories.UnitOfWork
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using ERP.Repositories.GenericRepository;
    using ERP.Entities.Models;

    public class UnitOfWork : IUnitOfWork
    {
        #region Fields
        private readonly ERPDbContext context;
        private Hashtable repositories;
        #endregion

        #region Constructors                
        public UnitOfWork(ERPDbContext context)
        {
            this.context = context;
        }

        #endregion
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
                return this.context.Database;
            }
            finally
            {
                this.repositories = null;
            }
        }

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

        public IRepository<T> Repository<T>() where T : class
        {
            if (this.repositories == null)
            {
                this.repositories = new Hashtable();
            }

            var type = typeof(T).Name;

            if (!this.repositories.ContainsKey(type))
            {
                Type repositoryType = typeof(ERP.Repositories.GenericRepository.Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), this.context);
                this.repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)this.repositories[type];
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.context.Dispose();
        }
        #endregion
    }
}
