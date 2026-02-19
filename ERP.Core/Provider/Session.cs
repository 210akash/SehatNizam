//-----------------------------------------------------------------------
// <copyright file="Session.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Core.Provider
{
    using System;

    /// <summary>
    /// Session class
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid LoggedInUserId { get; set; }

        /// <summary>
        /// Gets or sets the branch identifier.
        /// </summary>
        /// <value>
        /// The branch identifier.
        /// </value>
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public long DepartmentId { get; set; }
        public long StoreId { get; set; }
        public Guid[] RoleId { get; set; }
        public string[] Roles { get; set; }
    }
}
