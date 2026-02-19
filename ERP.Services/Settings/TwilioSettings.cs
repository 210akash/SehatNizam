//-----------------------------------------------------------------------
// <copyright file="TwilioSettings.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Settings
{
    /// <summary>
    /// TWILIO settings
    /// </summary>
    public class TwilioSettings
    {
        /// <summary>
        /// Gets or sets the account sid.
        /// </summary>
        /// <value>
        /// The account sid.
        /// </value>
        public string AccountSid { get; set; }

        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets from number.
        /// </summary>
        /// <value>
        /// From number.
        /// </value>
        public string FromNumber { get; set; }
    }
}
