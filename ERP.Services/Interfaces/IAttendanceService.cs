//-----------------------------------------------------------------------
// <copyright file="IAuthService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Authentication and authorization Interface
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Finds the by phone number asynchronous.
        /// </summary>
        /// <param name="fromDate">fromDate.</param>
        /// <param name="toDate">toDate.</param>
        /// <returns>return user</returns>
        Task<List<Tuple<long, string>>> SyncAttendanceByDate(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Finds the by phone number asynchronous.
        /// </summary>
        /// <param name="employeeId">employeeId.</param>
        /// <param name="fromDate">fromDate.</param>
        /// <param name="toDate">toDate.</param>
        /// <returns>return user</returns>
        Task<List<Tuple<long, string>>> SyncAttendanceByEmployeeAsync(string employeeId, DateTime fromDate, DateTime toDate);
    }
}
