//-----------------------------------------------------------------------
// <copyright file="AuthService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System;
    using ERP.Entities.Models;
    using ERP.Repositories.UnitOfWork;
    using ERP.Services.Interfaces;
    using Microsoft.Extensions.Logging;
    using ERP.Core.Provider;

    /// <summary>
    /// Authentication and authorization service
    /// </summary>
    public class LoggerService : ILoggerService
    {
        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly SessionProvider sessionProvider;

        public LoggerService(SessionProvider sessionProvider , IUnitOfWork unitOfWork)
        {
            this.sessionProvider = sessionProvider;
            this.unitOfWork = unitOfWork;
        }

        public void LogInformation(string Message,string Category,string Query, string Controller, string Action, Guid? UserId)
        {
            Log(LogLevel.Information, Message, Query, Controller, Action,UserId);
        }

        public void LogError(Exception ex, string Category, string Query, string Controller, string Action)
        {
            Log(LogLevel.Information, ex.Message, Query, Controller , Action,null);
        }

        public void Log(LogLevel logLevel, string message, string Query, string Controller, string Action, Guid? UserId)
        {
            Logs _Log = new Logs
            {
                LogLevel = logLevel.ToString(),
                Message = message,
                Query = Query,
                Controller = Controller,
                Action = Action,
                Created = DateTime.Now,
                UserId = UserId == null ? sessionProvider.Session.LoggedInUserId : UserId
            };

            unitOfWork.Repository<Logs>().AddAsync(_Log);
            unitOfWork.SaveChanges();
        }

    }
}