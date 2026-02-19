//-----------------------------------------------------------------------
// <copyright file="SendVoiceMessageCommand.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Mediator.Mediator.SMS.Command
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;

    /// <summary>
    /// Send voice message command
    /// </summary>
    public class SendVoiceMessageCommand : IRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendVoiceMessageCommand"/> class.
        /// </summary>
        /// <param name="toNumber">To number.</param>
        /// <param name="message">The message.</param>
         public SendVoiceMessageCommand(string toNumber, string message)
        {
            this.Message = message;
            this.ToNumber = toNumber;
        }

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