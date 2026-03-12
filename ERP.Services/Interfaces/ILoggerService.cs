//-----------------------------------------------------------------------
// <copyright file="IAuthService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System;

    /// <summary>
    /// Authentication and authorization Interface
    /// </summary>
    public interface ILoggerService 
    {
        public void LogInformation(string Message, string Category, string Query, string Controller ,string Action,Guid? UserId);
        public void LogError(Exception ex, string Category, string Query, string Controller, string Action);
    }
}
