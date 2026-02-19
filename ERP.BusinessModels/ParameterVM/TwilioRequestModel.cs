//-----------------------------------------------------------------------
// <copyright file="TwilioRequestModel.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ParameterVM
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TWILIO request model
    /// </summary>
    public class TwilioRequestModel
    {
        /// <summary>
        ///  Gets or sets the to number.
        /// </summary>
        /// <value>
        /// To number.
        /// </value>
        [Required(ErrorMessage = "To number is required")]
        public string ToNumber { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}
