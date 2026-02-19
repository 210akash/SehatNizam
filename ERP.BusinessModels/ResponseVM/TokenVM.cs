//-----------------------------------------------------------------------
// <copyright file="TokenVM.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using ERP.Entities.Migrations;
    using ERP.Entities.Models;
    using System;

    /// <summary>
    /// Token Model
    /// </summary>
    public class TokenVM
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is login success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is login success; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoginSuccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is two factor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is two factor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the profile BLOB url.
        /// </summary>
        /// <value>
        /// The profile BLOB url.
        /// </value>
        public string ProfileBlobURl { get; set; }

        /// <summary>
        /// Gets or sets Phone Number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the token provider.
        /// </summary>
        /// <value>
        /// The token provider.
        /// </value>
        public string TokenProvider { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }
        public string CompanyBlobUrl { get; set; }
        public string TimeZone { get; set; }
        public string GMT { get; set; }
        public string TimeZoneName { get; set; }

        public string Role { get; set; }
        public Guid? RoleId { get; set; }
        public string CompanyName { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public int? SecurityMehtod2FA { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string Code { get; set; }
        public long? DepartmentId { get; set; }
        public Department Department { get; set; }

        public long? StoreId { get; set; }
        public Entities.Models.Store Store { get; set; }
    }
}