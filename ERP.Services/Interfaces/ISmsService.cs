//-----------------------------------------------------------------------
// <copyright file="ISmsService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///  Email Service Interface
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends the SMS asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="number">The number.</param>
        /// <returns>return boolean value</returns>
        Task<bool> SendSMSAsync(string message, string number);

        /// <summary>
        /// Sends the voice message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="number">The number.</param>
        /// <returns>return boolean value.</returns>
        Task<bool> SendVoiceMessageAsync(string message, string number);
    }
}
