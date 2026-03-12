//-----------------------------------------------------------------------
// <copyright file="TokenLifetimeValidator.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Helpers
{
    using System;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Token Life Time Validator
    /// </summary>
    public class TokenLifetimeValidator
    {
        /// <summary>
        /// Validates the specified not before.
        /// </summary>
        /// <param name="notBefore">The not before.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="tokenToValidate">The token to validate.</param>
        /// <param name="param">The parameter.</param>
        /// <returns>return true or false</returns>
        public static bool Validate(
               DateTime? notBefore,
               DateTime? expires,
               SecurityToken tokenToValidate,
               TokenValidationParameters @param)
        {
            return expires != null && expires > DateTime.UtcNow;
        }
    }
}
