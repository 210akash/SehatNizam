//-----------------------------------------------------------------------
// <copyright file="SendGridEmailModel.cs" company="Aepistle">
//     copy right Aepistle.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.ParameterVM
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Declaration of Send Grid Email Model class.
    /// </summary>
    public class SendGridEmailModel
    {
        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        /// <value>
        /// The message subject.
        /// </value>
        public string MessageSubject { get; set; }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        /// <value>
        /// The message body.
        /// </value>
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets or sets the email buttons.
        /// </summary>
        /// <value>
        /// The email buttons.
        /// </value>
        public List<EmailModelButtons> EmailButtons { get; set; }

        #region Necessary Values For Send Grid

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the email to.
        /// </summary>
        /// <value>
        /// The email to.
        /// </value>
        public string EmailTo { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        /// <value>
        /// The email subject.
        /// </value>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets the name of the attachment file.
        /// </summary>
        /// <value>
        /// The name of the attachment file.
        /// </value>
        public string AttachmentFileName { get; set; }

        /// <summary>
        /// Gets or sets the attachment content stream.
        /// </summary>
        /// <value>
        /// The attachment content stream.
        /// </value>
        public Stream AttachmentContentStream { get; set; }

        /// <summary>
        /// Gets or sets the type of the attachment.
        /// </summary>
        /// <value>
        /// The type of the attachment.
        /// </value>
        public string AttachmentType { get; set; }

        #endregion
    }
}
