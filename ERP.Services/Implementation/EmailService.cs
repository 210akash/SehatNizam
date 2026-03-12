//-----------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ParameterVM;
    using ERP.Services.Helper;
    using ERP.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using ERP.Services.Settings;

    /// <summary>
    /// Email service for sending and receiving email
    /// </summary>
    public class EmailService : IEmailService
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
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="emailSetting">The email setting.</param>
        /// <param name="client">The client.</param>
        public EmailService(IOptions<EmailSetting> emailSetting, SendGridClient client)
        {
            this.emailSetting = emailSetting.Value;
            this.client = client;
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="buttons">The button list</param>
        /// <param name="cc">The cc.</param>
        /// <param name="messageSubject">The message subject.</param>
        /// <returns>Nothing return</returns>
        public async Task SendAsync(
            string toEmail,
            string subject,
            string body,
            List<EmailModelButtons> buttons,
            string templateName,
            string cc = ""
            )
        {
            var emailModel = new EmailModel()
            {
                FileName = templateName,
                EmailButtons = buttons,
                Body = body
            };

            var from = new EmailAddress(emailSetting.Email, emailSetting.EmailTitle);
            var to = new EmailAddress(toEmail);
            var plainTextContent = string.Empty;
            // var htmlContent = await RazorLightHelper.CompileTemplateAsync(emailModel);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>Nothing return</returns>
        public async Task SendAsync(SendGridEmailModel emailModel)
        {
            // Sending Email with SendGrid using Template:
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetGlobalSubject(emailModel.EmailSubject);
            sendGridMessage.AddTo(emailModel.EmailTo);
            sendGridMessage.SetSubject(emailModel.EmailSubject);
            sendGridMessage.SetFrom(new EmailAddress(emailSetting.Email, emailSetting.EmailTitle));
            sendGridMessage.SetTemplateId(emailModel.TemplateId);
            if (!string.IsNullOrEmpty(emailModel.AttachmentFileName) && emailModel.AttachmentContentStream != null)
            {
                await sendGridMessage.AddAttachmentAsync(emailModel.AttachmentFileName, emailModel.AttachmentContentStream, emailModel.AttachmentType);
            }

            //this.SetTemplateData(ref emailModel, ref sendGridMessage);
            await client.SendEmailAsync(sendGridMessage);

        }

        /// <summary>
        /// Sets the template data.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="sendGridMessage">The send grid message.</param>
        //private void SetTemplateData(ref SendGridEmailModel emailModel, ref SendGridMessage sendGridMessage)
        //{
        //    switch (emailModel.TemplateId)
        //    {
        //        case EmailTemplates.DeleteGroupTemplateId:
        //            sendGridMessage.SetTemplateData(new
        //            {
        //                message_Subject = emailModel.MessageSubject,
        //                message_Body = emailModel.MessageBody
        //            });
        //            break;

        //        case EmailTemplates.GenericEmailTemplateId:
        //            sendGridMessage.SetTemplateData(new
        //            {
        //                is_Message_Subject = !string.IsNullOrEmpty(emailModel.MessageSubject),
        //                message_Subject = emailModel.MessageSubject,
        //                message_Body = emailModel.MessageBody,
        //                emailButtons = emailModel.EmailButtons
        //            });
        //            break;
        //    }
        //}
    }
}