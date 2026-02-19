//-----------------------------------------------------------------------
// <copyright file="SendVoiceMessageHandler.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------


    using System.Threading;
    using System.Threading.Tasks;
    using ERP.Mediator.Mediator.SMS.Command;
    using MediatR;
    using Microsoft.Extensions.Options;
    using Twilio;
    using Twilio.Rest.Api.V2010.Account;
    using ERP.BusinessModels.Enums;
    using ERP.Services.Settings;

namespace ERP.Mediator.Mediator.SMS.Handler
{
    /// <summary>
    /// send SMS handler
    /// </summary>
    public class SendVoiceMessageHandler : IRequestHandler<SendVoiceMessageCommand, bool>
    {
        /// <summary>
        /// The TWILIO settings
        /// </summary>
        private readonly TwilioSettings twilioSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendVoiceMessageHandler"/> class.
        /// </summary>
        /// <param name="smsService">The SMS service.</param>
        public SendVoiceMessageHandler(IOptions<TwilioSettings> twilioSetting)
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
        public async Task<bool> Handle(SendVoiceMessageCommand request, CancellationToken cancellationToken)
        {
            await CallResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber(this.twilioSettings.FromNumber),
                to: new Twilio.Types.PhoneNumber(request.ToNumber),
                twiml: new Twilio.Types.Twiml(string.Format(Constants.AddPrivacyError, 2, request.Message)));

            return true;
        }
    }
}
