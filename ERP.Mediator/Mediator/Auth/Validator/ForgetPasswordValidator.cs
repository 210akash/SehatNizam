using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using ERP.BusinessModels.Enums;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Query;

namespace ERP.Mediator.Mediator.Auth.Validator
{
    public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordQuery>
    {
        private readonly ERPDbContext context;

        public ForgetPasswordValidator(ERPDbContext context)
        {
            this.context = context;
            var message = "Something went wrong.";
            RuleFor(x => x.EmailOrPhone).NotEmpty()
                .WithMessage("Email/Phone is required.");
            RuleFor(x => x).NotEmpty()
                .Must(x => this.IsValidRequest(x.IsEmail, x.EmailOrPhone, out message))
                .WithMessage(x => message);
        }

        public bool IsValidRequest(bool isEmail, string emailOrPhone, out string message)
        {
            message = string.Empty;
            emailOrPhone = emailOrPhone.Trim();

            if (isEmail)
            {
                if (!IsValidEmail(emailOrPhone))
                {
                    message = Constants.InvalidEmail;
                    return false;
                }
            }
            else
            {
                if (!IsValidPhoneNumber(emailOrPhone))
                {
                    message = Constants.InvalidPhoneNumber;
                    return false;
                }
            }

            var user = context.AspNetUsers.FirstOrDefault(s => s.Email == emailOrPhone && isEmail || s.PhoneNumber == emailOrPhone && !isEmail);

            if (user == null)
            {
                message = Constants.UserNotFound;
                return false;
            }

            if (!user.EmailConfirmed && isEmail)
            {
                message = Constants.EmailNotConfirmed;
                return false;
            }

            if (!user.PhoneNumberConfirmed && !isEmail)
            {
                message = Constants.PhoneNumberNotConfirmed;
                return false;
            }

            return true;
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            phoneNumber = phoneNumber.Trim();

            // Only Validate international Phone numbers with country code
            // Otherwise, We receive an error while sending message/call from twilio
            return Regex.Match(phoneNumber, @"^\+(?:[0-9] ?){6,14}[0-9]$").Success;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}

