//-----------------------------------------------------------------------
// <copyright file="SendVoiceMessageCommand.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------


    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ParameterVM;

namespace ERP.Mediator.Mediator.Email.Query
{
    /// <summary>
    /// Send voice message command
    /// </summary>
    public class SendEmailQuery : IRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendVoiceMessageCommand"/> class.
        /// </summary>
        /// <param name="toNumber">To number.</param>
        /// <param name="message">The message.</param>
         public SendEmailQuery(string toEmail,
            string subject,
            string body,
            List<EmailModelButtons> buttons,
            string templateName,
            string cc = ""
            )
        {
            this.EmailButtons = buttons;
            this.FileName = templateName;
            this.Body = body;
            this.ToEmail = toEmail;
            this.Subject = subject;
            this.CC = cc;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        /// <value>
        /// The base path.
        /// </value>
        public string BasePath { get; set; } = Constants.BasePath;

        /// <summary>
        /// Gets or sets the email buttons.
        /// </summary>
        /// <value>
        /// The email buttons.
        /// </value>
        public List<EmailModelButtons> EmailButtons { get; set; }

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        /// <value>
        /// The message subject.
        /// </value>
        public string MessageSubject { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject { get; set; }

        /// <summary>
        /// Converts to email.
        /// </summary>
        /// <value>
        /// To email.
        /// </value>
        public string ToEmail { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }
    }
}