//-----------------------------------------------------------------------
// <copyright file="ErrorHandlingMiddlewareExtensions.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Middleware
{
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Error Handling Middleware Extensions class
    /// </summary>
    public static class ErrorHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Uses the error handling middleware.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>Nothing return</returns>
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
