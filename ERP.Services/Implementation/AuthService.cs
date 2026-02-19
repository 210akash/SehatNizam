//-----------------------------------------------------------------------
// <copyright file="AuthService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ERP.BusinessModels.AttributeExtensions;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;
    using ERP.Repositories.UnitOfWork;
    using ERP.Services.Extensions;
    using ERP.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using AutoMapper;
    using System.Web;
    using Twilio;
    using Twilio.Rest.Lookups.V1;
    using ERP.Services.Settings;
    using Microsoft.Extensions.Options;
    using ERP.Services.Interfaces;
    using ERP.Core.Provider;

    /// <summary>
    /// Authentication and authorization service
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        /// <summary>
        /// Config declare
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// The email service
        /// </summary>
        private readonly IEmailService emailService;

        /// <summary>
        /// The SMS service
        /// </summary>
        private readonly ISmsService smsService;
        /// <summary>
        /// The SMS service
        /// </summary>
        private readonly TwilioSettings twilioSettings;

        /// <summary>
        /// The session provider
        /// </summary>
        private readonly SessionProvider sessionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="smsService">The SMS service.</param>
        public AuthService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, IEmailService emailService, ISmsService smsService, IOptions<TwilioSettings> twilioSetting, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
            this.emailService = emailService;
            this.smsService = smsService;
            this.twilioSettings = twilioSetting.Value;
            TwilioClient.Init(this.twilioSettings.AccountSid, this.twilioSettings.AuthToken);
            this.sessionProvider = sessionProvider;
        }

        /// <summary>
        /// Domains the mapper.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>return the string</returns>
        public static string DomainMapper(Match match)
        {
            // Use IdnMapping class to convert Unicode domain names.
            var idn = new IdnMapping();

            // Pull out and process domain name (throws ArgumentException on invalid)
            var domainName = idn.GetAscii(match.Groups[2].Value);

            return match.Groups[1].Value + domainName;
        }

        public async Task ChangeEmail(Guid userId, string email)
        {
            var getUser = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == userId);
            getUser.EmailConfirmationCode = this.GenerateRandomSixDigitCode();
            getUser.EmailCodeExpiryDateTime = DateTime.UtcNow.AddMinutes(30);
            this.unitOfWork.Repository<AspNetUsers>().Update(getUser);
            await this.unitOfWork.SaveChangesAsync();
            //// Send Confirmation email
            await this.emailService.SendAsync(
                email,
              Constants.EmailConfirmation,
              getUser.EmailConfirmationCode,
              new List<EmailModelButtons>(),
              Constants.EmailConfirmationTemplate,
              string.Empty);
        }

        /// <summary>
        /// Encrypts the specified to encrypt.
        /// </summary>
        /// <param name="toEncrypt">To encrypt.</param>
        /// <returns></returns>
        public string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            string key = "ThisIsTheKeyForOneCubeCRM=SecurityKey";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Decrypts the specified cipher string.
        /// </summary>
        /// <param name="cipherString">The cipher string.</param>
        /// <returns></returns>
        public string Decrypt(string cipherString)
        {
            cipherString = HttpUtility.UrlDecode(cipherString).Replace(" ", "+");
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace(" ", "+"));
            string key = "ThisIsTheKeyForOneCubeCRM=SecurityKey";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Resends the email confirmation code.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return the task</returns>
        public async Task ResendEmailConfirmationCode(Guid userId)
        {
            await this.SendEmailConfirmationCode(userId);
        }

        /// <summary>
        /// Checks the password asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>The task</returns>
        public async Task<bool> ChangePhoneNumberAsync(AspNetUsersModel user, string phoneNumber)
        {
            var code = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            if (await this.SendTwoFactorCodeViaSmsAsync(phoneNumber, code, true, false))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resends the phone number verification code asynchronous.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="isSmsMessage">if set to <c>true</c> [is SMS message].</param>
        /// <param name="isVoiceMessage">if set to <c>true</c> [is voice message].</param>
        /// <returns>
        /// return boolean value
        /// </returns>
        public async Task<bool> SendVerificationCodeOnMobileAsync(string code, string phoneNumber, bool isSmsMessage, bool isVoiceMessage)
        {
            if (await this.SendTwoFactorCodeViaSmsAsync(phoneNumber, code, isSmsMessage, isVoiceMessage))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sends the two factor code via SMS asynchronous.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="code">The code.</param>
        /// <param name="isSmsMessage">if set to <c>true</c> [is SMS message].</param>
        /// <param name="isVoiceMessage">if set to <c>true</c> [is voice message].</param>
        /// <returns>the task</returns>
        private async Task<bool> SendTwoFactorCodeViaSmsAsync(string phoneNumber, string code, bool isSmsMessage, bool isVoiceMessage)
        {
            var isSuccess = false;
            var twilioRequestModel = new TwilioRequestModel()
            {
                ToNumber = phoneNumber,
                Message = string.Format("Your verification code is {0}", code),
            };

            if (isSmsMessage)
            {
                isSuccess = await this.smsService.SendSMSAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
            }

            if (isVoiceMessage)
            {
                isSuccess = await this.smsService.SendVoiceMessageAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
            }

            return isSuccess;
        }

        /// <summary>
        /// Finds the by phone number asynchronous.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>return user</returns>
        public async Task<AspNetUsersModel> FindByPhoneNumberAsync(string phoneNumber)
        {
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.PhoneNumber == phoneNumber);
            return this.mapper.Map<AspNetUsersModel>(user);
        }

        /// <summary>
        /// Updates the authentication type setting identifier.
        /// </summary>
        /// <param name="userId">the user identifier</param>
        /// <param name="code">the code</param>
        /// <returns>
        /// return boolean value
        /// </returns>
        public async Task<string> ConfirmEmailAsync(Guid userId, string code)
        {
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(s => s.Id == userId);
            if (user == null)
            {
                return Constants.InvalidUserId;
            }

            if (user.EmailCodeExpiryDateTime < DateTime.UtcNow)
            {
                return Constants.EmailConfirmationCodeExpired;
            }

            if (!user.EmailConfirmationCode.Equals(code))
            {
                return Constants.InvalidEmailConfirmationCode;
            }

            bool lockTaken = false;
            Monitor.Enter(user, ref lockTaken);
            try
            {
                user.EmailConfirmed = true;
                this.unitOfWork.Repository<AspNetUsers>().Update(user);
                this.unitOfWork.SaveChanges();
                ////this.unitOfWork.Repository<AspNetUsers>().DetachEntry(user);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(user);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Changes the email asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The code.</param>
        /// <param name="email">The email.</param>
        /// <returns>
        /// return response information
        /// </returns>
        public async Task<List<ResponseInfos>> ChangeEmailAsync(Guid userId, string code, string email)
        {
            var responseInfos = new List<ResponseInfos>();
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(s => s.Id == userId);
            if (user == null)
            {
                responseInfos.AddError(Constants.InvalidUserId);
                return responseInfos;
            }

            if (user.EmailCodeExpiryDateTime < DateTime.UtcNow)
            {
                responseInfos.AddError(Constants.EmailConfirmationCodeExpired);
                return responseInfos;
            }

            if (!user.EmailConfirmationCode.Equals(code))
            {
                responseInfos.AddError(Constants.InvalidEmailConfirmationCode);
                return responseInfos;
            }

            bool lockTaken = false;
            Monitor.Enter(user, ref lockTaken);
            try
            {
                user.Email = email.ToUpperInvariant();
                user.NormalizedEmail = email;
                user.NormalizedUserName = email;
                this.unitOfWork.Repository<AspNetUsers>().Update(user);
                this.unitOfWork.SaveChanges();
                ////this.unitOfWork.Repository<AspNetUsers>().DetachEntry(user);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(user);
                }
            }

            return responseInfos;
        }

        /// <summary>
        /// Determines whether [is valid email] [the specified email address].
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>
        ///   <c>true</c> if [is valid email] [the specified email address]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            email = email.Trim();

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                var regExp = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
                return Regex.IsMatch(
                                     email,
                                     regExp,
                                     RegexOptions.IgnoreCase,
                                     TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is valid phone number] [the specified phone number].
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>
        ///   <c>true</c> if [is valid phone number] [the specified phone number]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            var type = new List<string> {
            "carrier"
            };
            PhoneNumberResource.Fetch(
                type: type,
                pathPhoneNumber: new Twilio.Types.PhoneNumber(phoneNumber)
            );
            return true;
        }

        /// <summary>
        /// Determines whether [is valid phone number] [the specified phone number].
        /// </summary>
        /// <param name="countryCode">The phone number.</param>
        /// <returns>
        ///   <c>true</c> if [is valid country code] [the specified phone number]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) && string.IsNullOrEmpty(countryCode))
            {
                return false;
            }

            countryCode = countryCode.Trim();

            // Only Validate international Phone numbers with country code
            // Otherwise, We receive an error while sending message/call from twilio
            return Regex.Match(countryCode, @"^\+(\d{1}\-)?(\d{1,3})$").Success;
        }

        /// <summary>
        /// Determines whether [is phone number already exists] [the specified phone number].
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>
        ///   <c>true</c> if [is phone number already exists] [the specified phone number]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsPhoneNumberAlreadyExists(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            phoneNumber = phoneNumber.Trim();

            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAllAsync(x => x.PhoneNumber.Contains(phoneNumber));
            if (user == null || user.Count() <= 0)
            {
                return false;
            }

            if (user.Count() > 1)
            {
                return true;
            }

            // If user is not null, then phone number already associated with an account in DB.
            return user.FirstOrDefault().PhoneNumberConfirmed;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return the full name</returns>
        public string GetFullName(Guid userId)
        {
            var user = this.unitOfWork.Repository<AspNetUsers>().Find(x => x.Id == userId);
            if (user != null)
            {
                return user.FirstName + " " + user.LastName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates the random six digit code.
        /// </summary>
        /// <returns>return code</returns>
        private string GenerateRandomSixDigitCode()
        {
            var generator = new Random();
            string r = generator.Next(0, 999999).ToString("D6");
            return r;
        }

        /// <summary>
        /// Sends the email confirmation code.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return the task</returns>
        private async Task SendEmailConfirmationCode(Guid userId)
        {
            var getUser = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.Id == userId);
            getUser.EmailConfirmationCode = this.GenerateRandomSixDigitCode();
            getUser.EmailCodeExpiryDateTime = DateTime.UtcNow.AddMinutes(30);
            this.unitOfWork.Repository<AspNetUsers>().Update(getUser);
            await this.unitOfWork.SaveChangesAsync();
            //// this.unitOfWork.Repository<AspNetUsers>().DetachEntry(getUser);

            //// Send Confirmation email
            await this.emailService.SendAsync(
               getUser.Email,
               Constants.REGSUCCSS,
               string.Format("{0}{1}", Constants.REGBODY, string.Format(Constants.ConfirmationCodeId, getUser.EmailConfirmationCode)),
               new List<EmailModelButtons>(),
               string.Empty,
               string.Format("Welcome {0} {1}", getUser.FirstName, getUser.LastName));
        }
    }
}