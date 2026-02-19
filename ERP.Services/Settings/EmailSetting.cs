//-----------------------------------------------------------------------
// <copyright file="EmailSetting.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Settings
{
    /// <summary>
    /// Email Setting Implementation 
    /// </summary>
    public class EmailSetting
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the send grid API key.
        /// </summary>
        /// <value>
        /// The send grid API key.
        /// </value>
        public string SendGridApiKey { get; set; }

        /// <summary>
        /// Gets or sets the email title.
        /// </summary>
        /// <value>
        /// The email title.
        /// </value>
        public string EmailTitle { get; set; }
    }
}
