using ERP.BusinessModels.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ParameterVM
{
    /// <summary>
    /// Email model class
    /// </summary>
    public class EmailModel
    {

        public EmailModel()
        {
            EmailButtons  = new List<EmailModelButtons>();
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
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }
    }
}
