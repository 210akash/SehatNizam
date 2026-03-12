//-----------------------------------------------------------------------
// <copyright file="CheckPasswordHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Extensions;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// Check Password Handler
    /// </summary>
    /// <seealso cref="TransferMaster.Mediator.Mediator.BaseHandler" />
    /// <seealso cref="IRequestHandler{TransferMaster.Mediator.Mediator.Auth.Command.Send2FAOTPCommand, TransferMaster.BusinessModels.ResponseVM.TokenVM}" />
    public class Send2FAOTPHandler : BaseHandler, IRequestHandler<Send2FAOTPCommand, bool>
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

        private readonly IEmailService emailService;
        private readonly ISmsService smsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public Send2FAOTPHandler(IMapper mapper, IEmailService emailService, ISmsService smsService, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.smsService = smsService;
            this.userManager = userManager;
            this.config = config;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(Send2FAOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var emailModel = new OTPModel()
            {
                Name = user.FirstName + " " + user.LastName,
                Token = token
            };

            if (request.IsTokenSendToEmail)
                emailService.SendAsync(user.NormalizedEmail, "OTP", null, new List<EmailModelButtons>() { new EmailModelButtons() { BtnName = user.FirstName + " " + user.LastName, BtnLink = token } }, Constants.VerifyOTPTemplate);

            if (request.IsTokenSendToPhonoNo && !string.IsNullOrWhiteSpace(request.PhonoNo))
            {
                if (!user.PhoneNumber.StartsWith("+"))
                {
                    user.PhoneNumber = "+" + user.PhoneNumber;
                }

                var twilioRequestModel = new TwilioRequestModel()
                {
                    ToNumber = user.PhoneNumber,
                    Message = "Your OTP code is " + token,
                };
                smsService.SendSMSAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
            }
            return true;
        }

    }
}