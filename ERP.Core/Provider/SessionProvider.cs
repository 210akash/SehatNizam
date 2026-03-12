//-----------------------------------------------------------------------
// <copyright file="SessionProvider.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Core.Provider
{
    using System;

    /// <summary>
    /// Session provider
    /// </summary>
    public class SessionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionProvider"/> class.
        /// </summary>
        public SessionProvider()
        {
            this.Session = new Session();
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public Session Session { get; set; }
    }
}