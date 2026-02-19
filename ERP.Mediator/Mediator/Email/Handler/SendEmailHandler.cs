//-----------------------------------------------------------------------
// <copyright file="SendEmailHandler.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------


using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.SMS.Command;
using MediatR;
using Microsoft.Extensions.Options;
using Twilio;
using ERP.Services.Settings;
using Twilio.Rest.Api.V2010.Account;
using ERP.BusinessModels.Enums;
using ERP.Mediator.Mediator.Email.Query;
using ERP.BusinessModels.ParameterVM;
using SendGrid.Helpers.Mail;
using SendGrid;
using ERP.Services.Helper;

namespace ERP.Mediator.Mediator.Email.Handler
{
    /// <summary>
    /// send SMS handler
    /// </summary>
    public class SendEmailHandler : IRequestHandler<SendEmailQuery, bool>
    {
        /// <summary>
        /// Email configuration settings
        /// </summary>
        private readonly EmailSetting emailSetting;

        /// <summary>
        /// The client
        /// </summary>
        private readonly SendGridClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailHandler"/> class.
        /// </summary>
        /// <param name="smsService">The SMS service.</param>
        public SendEmailHandler(IOptions<EmailSetting> emailSetting, SendGridClient client)
        {
            this.emailSetting = emailSetting.Value;
            this.client = client;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(SendEmailQuery request, CancellationToken cancellationToken)
        {
            var emailModel = new EmailModel()
            {
                FileName = request.FileName,
                EmailButtons = request.EmailButtons,
                Body = request.Body
            };

            var from = new EmailAddress(emailSetting.Email, emailSetting.EmailTitle);
            var to = new EmailAddress(request.ToEmail);
            var plainTextContent = string.Empty;
            //  var htmlContent = await RazorLightHelper.CompileTemplateAsync(emailModel);
            var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, plainTextContent, "");
            var a = await client.SendEmailAsync(msg);
            return true;
        }
    }
}
