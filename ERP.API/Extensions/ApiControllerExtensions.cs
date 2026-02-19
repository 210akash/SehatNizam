//-----------------------------------------------------------------------
// <copyright file="ApiControllerExtensions.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using ERP.BusinessModels.Enums;

    /// <summary>
    /// The API Controller Extensions
    /// </summary>
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Results the specified status.
        /// </summary>
        /// <typeparam name="T">Any entity model data</typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="status">The status.</param>
        /// <param name="data">The data.</param>
        /// <param name="message">The message.</param>
        /// <returns>The action result</returns>
        public static ActionResult Result<T>(this ControllerBase controller, ResponseStatus status, T data = default, string message = null)
        {
            return new Response<T>()
            {
                Status = status,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Results the specified status.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="status">The status.</param>
        /// <param name="data">The data.</param>
        /// <param name="message">The message.</param>
        /// <returns>The action result</returns>
        public static ActionResult Result(this ControllerBase controller, ResponseStatus status, object data = null, string message = null)
        {
            return new Response<object>()
            {
                Status = status,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Gets the logged in user identifier.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <returns>Returns the Logged In User Id</returns>
        public static Guid GetLoggedInUserId(this ControllerBase controller, IHttpContextAccessor httpContextAccessor)
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue("UserId");
            return !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : Guid.Empty;
        }

        /// <summary>
        /// Gets the logged in user session identifier.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <returns>Returns the Logged In User Id</returns>
        public static Guid GetLoggedInUserSessionId(this ControllerBase controller, IHttpContextAccessor httpContextAccessor)
        {
            var sessionId = httpContextAccessor.HttpContext.User.FindFirstValue("SessionId");
            return !string.IsNullOrEmpty(sessionId) ? Guid.Parse(sessionId) : Guid.Empty;
        }

        /// <summary>
        /// Gets the model validation errors.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>return error list</returns>
        public static string GetModelValidationErrors(this ControllerBase controller, ModelStateDictionary keyValuePairs)
        {
            var errors = keyValuePairs.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage);
            errors = errors.Distinct().ToList();
            var error = string.Join(",", errors.ToArray());
            return error;
        }

        /// <summary>
        /// Gets the identity response errors.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="identityResult">The identity result.</param>
        /// <returns>the identity response errors</returns>
        public static string GetIdentityResponseErrors(this ControllerBase controller, IdentityResult identityResult)
        {
            var errors = identityResult.Errors.Select(x => x.Description);
            errors = errors.Distinct().ToList();
            var error = string.Join(",", errors.ToArray());
            return error;
        }
    }
}