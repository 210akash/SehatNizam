//-----------------------------------------------------------------------
// <copyright file="SendSMSHandler.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------


    using System.Threading;
    using System.Threading.Tasks;
    using ERP.Mediator.Mediator.SMS.Command;
    using MediatR;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio;
    using Microsoft.Extensions.Options;
    using ERP.Services.Settings;

namespace ERP.Mediator.Mediator.SMS.Handler
{
    /// <summary>
    /// send SMS handler
    /// </summary>
    public class SendSMSHandler : IRequestHandler<SendSMSCommand, bool>
    {
        /// <summary>
        /// The TWILIO settings
        /// </summary>
        private readonly TwilioSettings twilioSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSMSHandler"/> class.
        /// </summary>
        /// <param name="smsService">The SMS service.</param>
        public SendSMSHandler(IOptions<TwilioSettings> twilioSetting)
        {
            this.twilioSettings = twilioSetting.Value;
            TwilioClient.Init(this.twilioSettings.AccountSid, this.twilioSettings.AuthToken);
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(SendSMSCommand request, CancellationToken cancellationToken)
        {
            await MessageResource.CreateAsync(
                body: request.Message,
                from: new Twilio.Types.PhoneNumber(this.twilioSettings.FromNumber),
                to: new Twilio.Types.PhoneNumber(request.ToNumber));

            return true;
        }
    }
}
