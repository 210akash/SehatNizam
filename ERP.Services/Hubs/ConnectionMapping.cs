//-----------------------------------------------------------------------
// <copyright file="ConnectionMapping.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Connection Mapping
    /// </summary>
    /// <typeparam name="T">the generic type parameter</typeparam>
    public class ConnectionMapping<T>
    {
        /// <summary>
        /// The signalConnections
        /// </summary>
        private readonly Dictionary<T, HashSet<string>> connections =
            new Dictionary<T, HashSet<string>>();

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                return this.connections.Count;
            }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="connectionId">The connection identifier.</param>
        public void Add(T key, string connectionId)
        {
            lock (this.connections)
            {
                HashSet<string> signalConnections;
                if (!this.connections.TryGetValue(key, out signalConnections))
                {
                    signalConnections = new HashSet<string>();
                    this.connections.Add(key, signalConnections);
                }

                lock (signalConnections)
                {
                    signalConnections.Add(connectionId);
                }
            }
        }

        /// <summary>
        /// Gets the signalConnections.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>the list of signalConnections</returns>
        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> signalConnections;
            if (this.connections.TryGetValue(key, out signalConnections))
            {
                return signalConnections;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets all connected users.
        /// </summary>
        /// <returns>the list of connected users</returns>
        public IEnumerable<Guid> GetAllConnectedUsers()
        {
            return this.connections.Select(d => Guid.Parse(d.Key.ToString()));
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="connectionId">The connection identifier.</param>
        public void Remove(T key, string connectionId)
        {
            lock (this.connections)
            {
                HashSet<string> signalConnections;
                if (!this.connections.TryGetValue(key, out signalConnections))
                {
                    return;
                }

                lock (signalConnections)
                {
                    signalConnections.Remove(connectionId);

                    if (signalConnections.Count == 0)
                    {
                        this.connections.Remove(key);
                    }
                }
            }
        }
    }
}
