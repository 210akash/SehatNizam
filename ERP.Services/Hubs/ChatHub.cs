using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ERP.Services.Hubs
{
    public class ChatHub : Hub
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
