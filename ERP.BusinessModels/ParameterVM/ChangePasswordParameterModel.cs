//-----------------------------------------------------------------------
// <copyright file="ChangePasswordParameterModel.cs" company="CRM">
//     copy right CRM.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ParameterVM
{
    using System;

    /// <summary>
    /// Change Password Parameter Model class.
    /// </summary>
    public class ChangePasswordParameterModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        /// <value>
        /// The old password.
        /// </value>
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets creates new password.
        /// </summary>
        /// <value>
        /// The new password.
        /// </value>
        public string NewPassword { get; set; }
    }
}