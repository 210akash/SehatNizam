//-----------------------------------------------------------------------
// <copyright file="CommentsHub.cs" company="sensyrtech">
//     transfercopy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Hubs
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System.Linq;

    public class NotificationHub : Hub
    {
        /// <summary>
        /// The Connections
        /// </summary>
        public static readonly ConnectionMapping<string> Connections = new ConnectionMapping<string>();

        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.
        /// </returns>
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (userId != null)
            {
                Connections.Add(userId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a connection with the hub is terminated.
        /// </summary>
        /// <param name="exception">the exception</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous disconnect.
        /// </returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (userId != null)
            {
                Connections.Remove(userId, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
