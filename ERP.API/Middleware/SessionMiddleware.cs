//-----------------------------------------------------------------------
// <copyright file="SessionMiddleware.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Middleware
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using ERP.Core.Provider;
    using System.Linq;

    /// <summary>
    /// Session Middleware
    /// </summary>
    public class SessionMiddleware
    {
        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <exception cref="ArgumentNullException">the next</exception>
        public SessionMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionProvider">The session provider.</param>
        /// <returns>return nothing</returns>
        public async Task Invoke(
            HttpContext context,
            SessionProvider sessionProvider)
        {
            var userId = context.User.FindFirstValue("UserId");
            var branchId = context.User.FindFirstValue("BranchId");
            var companyId = context.User.FindFirstValue("CompanyId");
            var departmentId = context.User.FindFirstValue("DepartmentId");
            var storeId = context.User.FindFirstValue("StoreId");
            var roleIdsClaim = context.User.FindFirstValue("RoleIds"); // This will hold multiple roles (e.g., comma-separated or JSON array)
            var roleNamesClaim = context.User.FindFirstValue("RoleNames"); // This will hold multiple roles (e.g., comma-separated or JSON array)

            if (userId != null)
            {
                sessionProvider.Session.LoggedInUserId = Guid.Parse(userId);
            }

            if (!string.IsNullOrEmpty(branchId))
            {
                sessionProvider.Session.BranchId = Convert.ToInt32(branchId);
            }

            if (companyId != null)
            {
                sessionProvider.Session.CompanyId = Convert.ToInt32(companyId);
            }

            // Handle multiple roles from "RoleIds" claim
            if (!string.IsNullOrEmpty(roleIdsClaim))
            {
                // Assuming roleIdsClaim is a comma-separated string, e.g., "roleGuid1,roleGuid2,roleGuid3"
                var roleIds = roleIdsClaim.Split(',')
                    .Where(roleId => Guid.TryParse(roleId, out _)) // Ensure that each part is a valid GUID
                    .Select(Guid.Parse)
                    .ToArray();

                // Assign the array of GUIDs to the session property
                sessionProvider.Session.RoleId = roleIds;

                var rolenames = roleNamesClaim.Split(',')
               .ToArray();

                sessionProvider.Session.Roles = rolenames;
            }

            if (departmentId != null)
            {
                sessionProvider.Session.DepartmentId = Convert.ToInt32(departmentId);
            }

            if (storeId != null)
            {
                sessionProvider.Session.StoreId = Convert.ToInt32(storeId);
            }

            await this.next(context);
        }
    }
}