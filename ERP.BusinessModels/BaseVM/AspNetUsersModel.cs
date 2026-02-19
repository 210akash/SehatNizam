//-----------------------------------------------------------------------
// <copyright file="AspNetUsersModel.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.BaseVM
{
    using ERP.Entities.Models;
    using System;

    /// <summary>
    /// Declaration of Asp Net Users Model class.
    /// </summary>
    public class AspNetUsersModel : BaseEntityModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the normalized user.
        /// </summary>
        /// <value>
        /// The name of the normalized user.
        /// </value>
        public string NormalizedUserName { get; set; }

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
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email.
        /// </summary>
        /// <value>
        /// The normalized email.
        /// </value>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [email confirmed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [email confirmed]; otherwise, <c>false</c>.
        /// </value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the security stamp.
        /// </summary>
        /// <value>
        /// The security stamp.
        /// </value>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Gets or sets the concurrency stamp.
        /// </summary>
        /// <value>
        /// The concurrency stamp.
        /// </value>
        public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Phone Number Confirmed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Phone Number Confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Two Factor Enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Two Factor Enabled; otherwise, <c>false</c>.
        /// </value>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the lockout end.
        /// </summary>
        /// <value>
        /// The lockout end.
        /// </value>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has Lockout Enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has Two Lockout Enabled; otherwise, <c>false</c>.
        /// </value>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        /// <value>
        /// The user role id.
        /// </value>
        public Guid[]? RoleId { get;set; }
        public string RoleName { get;set; }

        public bool IsRemember { get; set; }

        /// <summary>
        /// Gets or sets the access failed count.
        /// </summary>
        /// <value>
        /// The access failed count.
        /// </value>
        public int AccessFailedCount { get; set; }
        public string TimeZone { get; set; }
        public string Title { get; set; }
        public string ProfileBlobUrl { get; set; }
        public bool IsActive { get; set; }
        public bool HaveAssetAccess { get; set; }
        public int? SecurityMehtod2FA { get; set; }
        public bool? IsDeviceWizardComplete { get; set; }
        public string Code { get; set; }
        public long? DepartmentId { get; set; }
        public Department Department { get; set; }

        public long? StoreId { get; set; }
        public Store Store { get; set; }
    }
}