//-----------------------------------------------------------------------
// <copyright file="SendSMSCommand.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Mediator.Mediator.SMS.Command
{
    using MediatR;

    /// <summary>
    /// Send SMS command
    /// </summary>
    /// <seealso cref="MediatR.IRequest{System.Boolean}" />
    public class SendSMSCommand : SendVoiceMessageCommand, IRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSMSCommand"/> class.
        /// </summary>
        /// <param name="toNumber">To number.</param>
        /// <param name="message">The message.</param>
        public SendSMSCommand(string toNumber, string message)
            :base(toNumber, message)
        {
        }
    }
}
