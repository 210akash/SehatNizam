//-----------------------------------------------------------------------
// <copyright file="SmsService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System.Threading.Tasks;
    using ERP.BusinessModels.Enums;
    using ERP.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Twilio;
    using Twilio.Rest.Api.V2010.Account;
    using ERP.Services.Settings;
    using ERP.Services.Interfaces;

    /// <summary>
    /// Email service for sending and receiving email
    /// </summary>
    public class SmsService : ISmsService
    {
        /// <summary>
        /// The TWILIO settings
        /// </summary>
        private readonly TwilioSettings twilioSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsService"/> class.
        /// </summary>
        /// <param name="twilioSetting">The TWILIO setting.</param>
        public SmsService(IOptions<TwilioSettings> twilioSetting)
        {
            this.twilioSettings = twilioSetting.Value;
            TwilioClient.Init(this.twilioSettings.AccountSid, this.twilioSettings.AuthToken);
        }

        /// <summary>
        /// Sends the SMS asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="number">The number.</param>
        /// <returns>return boolean value</returns>
        public async Task<bool> SendSMSAsync(string message, string number)
        {
            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(this.twilioSettings.FromNumber),
                to: new Twilio.Types.PhoneNumber(number));

            return true;
        }

        /// <summary>
        /// Sends the voice message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="number">The number.</param>
        /// <returns>return boolean value</returns>
        public async Task<bool> SendVoiceMessageAsync(string message, string number)
        {
            await CallResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber(this.twilioSettings.FromNumber),
                to: new Twilio.Types.PhoneNumber(number),
                twiml: new Twilio.Types.Twiml(string.Format(Constants.VoiceMessage, 1, message, 2)));

            return true;
        }
    }
}