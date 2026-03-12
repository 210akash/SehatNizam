using MediatR;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class ForgetPasswordQuery : IRequest<AspNetUsersModel>
    {
        /// <summary>
        /// Gets or sets the email or phone.
        /// </summary>
        /// <value>
        /// The email or phone.
        /// </value>
        public string EmailOrPhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is email.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is email; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmail { get; set; } = true;
    }
}
