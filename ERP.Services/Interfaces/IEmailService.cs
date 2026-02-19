//-----------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ERP.BusinessModels.ParameterVM;

    /// <summary>
    ///  Email Service Interface
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="messageSubject">The message subject.</param>
        /// <returns>return nothings</returns>
        Task SendAsync(
            string toEmail,
            string subject,
            string body,
            List<EmailModelButtons> buttons,
            string tamplateName,
            string cc = ""
            );

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>return Nothing</returns>
        Task SendAsync(SendGridEmailModel emailModel);
    }
}
