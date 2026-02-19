//-----------------------------------------------------------------------
// <copyright file="DuplicateFoundException.cs" company="CRM">
//     CRM copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Exceptions
{
    using System;

    /// <summary>
    /// Entity Not Found Exception
    /// </summary>
    public class GeneralException : Exception
    {
        public GeneralException(string message) : base(message)
        {
        }
    }
}
