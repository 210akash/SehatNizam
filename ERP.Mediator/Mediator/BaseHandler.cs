//-----------------------------------------------------------------------
// <copyright file="BaseHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


    using ERP.Core.Provider;

namespace ERP.Mediator.Mediator
{
    /// <summary>
    /// Base handler
    /// </summary>
    public class BaseHandler
    {
        /// <summary>
        /// The session provider
        /// </summary>
        internal readonly SessionProvider SessionProvider;
        private Core.Provider.SessionProvider sessionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHandler"/> class.
        /// </summary>
        /// <param name="sessionProvider">The session provider.</param>
        public BaseHandler(SessionProvider sessionProvider)
        {
            this.SessionProvider = sessionProvider;
        }
    }
}
