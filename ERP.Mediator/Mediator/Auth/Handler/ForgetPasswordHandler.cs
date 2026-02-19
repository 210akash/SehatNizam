//-----------------------------------------------------------------------
// <copyright file="ForgetPasswordHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Mediator.Mediator.Auth.Validator;
using ERP.Mediator.Mediator.Email.Query;
using ERP.Mediator.Mediator.SMS.Command;
using ERP.Repositories.UnitOfWork;
using System.Collections.Generic;
using ERP.BusinessModels.ParameterVM;
using ERP.Services.Interfaces;
using Microsoft.Extensions.Configuration;
///using CRM.Mediator.Mediator.Member.Validator;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// Check Password Handler
    /// </summary>
    /// <seealso cref="ERP.Mediator.Mediator.BaseHandler" />
    /// <seealso cref="IRequestHandler{ERP.Mediator.Mediator.Auth.Command.LoginCommand, ERP.BusinessModels.ResponseVM.TokenVM}" />
    public class ForgetPasswordHandler : BaseHandler, IRequestHandler<ForgetPasswordQuery, AspNetUsersModel>
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
        /// The mediator
        /// </summary>
        private readonly IMediator mediator;

        /// <summary>
        /// The validator
        /// </summary>
        private readonly ForgetPasswordValidator validator;

        /// <summary>
        /// configuration
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        private readonly IAuthService authService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ForgetPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public ForgetPasswordHandler(IMapper mapper,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            UserManager<AspNetUsersModel> userManager,
            ForgetPasswordValidator validator,
            SessionProvider sessionProvider,
            IConfiguration config,
            IAuthService authService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validator = validator;
            this.userManager = userManager;
            this.mediator = mediator;
            this.authService = authService;
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
        public async Task<AspNetUsersModel> Handle(ForgetPasswordQuery request, CancellationToken cancellationToken)
        {
            validator.ValidateAndThrow(request);
            AspNetUsersModel user;
            request.EmailOrPhone = request.EmailOrPhone.Trim();
            if (request.IsEmail)
            {
                user = await userManager.FindByEmailAsync(request.EmailOrPhone);
            }
            else
            {
                var aspUser = await unitOfWork.Repository<AspNetUsers>().FindAsync(x => x.PhoneNumber == request.EmailOrPhone);
                user = await userManager.FindByEmailAsync(aspUser.Email);
            }

            var baseUrl = string.Format("{0}/auth/reset?token=", config["WebApplication:LiveUrl"]);
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var encryptLink = authService.Encrypt($"{user.Id},{code}");
            if (request.IsEmail)
            {
                await mediator.Send(new SendEmailQuery(
                request.EmailOrPhone,
                Constants.ResetEmailSubject,
                string.Format("{0} {1}", user.FirstName, user.LastName),
                new List<EmailModelButtons>() { new EmailModelButtons() { BtnName = "Reset My Password", BtnLink = string.Concat(baseUrl, encryptLink) } },
                Constants.ForgotPassworTemplate,
                string.Empty
                ));
            }
            else
            {
                await mediator.Send(new SendSMSCommand(request.EmailOrPhone, string.Format("Your verification code is {0}", code)));
            }
            return user;
        }
    }
}