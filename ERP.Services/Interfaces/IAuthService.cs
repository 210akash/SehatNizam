//-----------------------------------------------------------------------
// <copyright file="IAuthService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Authentication and authorization Interface
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Finds the by phone number asynchronous.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>return user</returns>
        Task<AspNetUsersModel> FindByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Determines whether [is valid phone number] [the specified phone number].
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>
        ///   <c>true</c> if [is valid phone number] [the specified phone number]; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidPhoneNumber(string phoneNumber);

        /// <summary>
        /// Determines whether [is phone number already exists] [the specified phone number].
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>
        ///   <c>true</c> if [is phone number already exists] [the specified phone number]; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsPhoneNumberAlreadyExists(string phoneNumber);

        /// <summary>
        /// Determines whether [is valid email] [the specified email address].
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>
        ///   <c>true</c> if [is valid email] [the specified email address]; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidEmail(string emailAddress);

        /// <summary>
        /// Changes the phone number asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>The Task</returns>
        Task<bool> ChangePhoneNumberAsync(AspNetUsersModel user, string phoneNumber);

        /// <summary>
        /// Resends the phone number verification code asynchronous.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="isSmsMessage">if set to <c>true</c> [is SMS message].</param>
        /// <param name="isVoiceMessage">if set to <c>true</c> [is voice message].</param>
        /// <returns>return boolean value</returns>
        Task<bool> SendVerificationCodeOnMobileAsync(string code, string phoneNumber, bool isSmsMessage, bool isVoiceMessage);

        /// <summary>
        /// Confirms the email asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The code.</param>
        /// <returns>return message type</returns>
        Task<string> ConfirmEmailAsync(Guid userId, string code);

        /// <summary>
        /// Resends the email confirmation code.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return the task</returns>
        Task ResendEmailConfirmationCode(Guid userId);

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return the full name</returns>
        string GetFullName(Guid userId);

        /// <summary>
        /// Changes the email asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The code.</param>
        /// <param name="email">The email.</param>
        /// <returns>
        /// return response information
        /// </returns>
        Task<List<ResponseInfos>> ChangeEmailAsync(Guid userId, string code, string email);

        /// <summary>
        /// Valid Country Code
        /// </summary>
        /// <param name="countryCode">The Country code.</param>
        /// <returns>return valid country code</returns>
        bool IsValidCountryCode(string countryCode);
        Task ChangeEmail(Guid userId, string email);
        string Encrypt(string toEncrypt);
        string Decrypt(string cipherString);
    }
}
