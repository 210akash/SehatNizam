using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Services.Hubs
{
    public class CommentHub : Hub
    {
        /// <summary>
        /// The Connections
        /// </summary>
        public static readonly ConnectionMapping<string> commentConnections = new ConnectionMapping<string>();

        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.
        /// </returns>
        public override Task OnConnectedAsync()
        {
            var username = Context.User.Claims.Where(m => m.Type == "UserId").Select(m => m.Value).FirstOrDefault();
            if (username != null)
            {
                commentConnections.Add(username, Context.ConnectionId);
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
            var username = Context.User.Claims.Where(m => m.Type == "UserId").Select(m => m.Value).FirstOrDefault();
            if (username != null)
            {
                commentConnections.Remove(username, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
