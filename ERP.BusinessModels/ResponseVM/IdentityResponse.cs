//-----------------------------------------------------------------------
// <copyright file="IdentityResponse.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ResponseVM
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Identity Response
    /// </summary>
    public class IdentityResponse : IdentityResult
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string Error { get; set; }
    }
}