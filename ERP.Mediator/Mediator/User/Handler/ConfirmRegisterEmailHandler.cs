using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.ComplexTypes;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.User.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Exceptions;
using ERP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class ConfirmRegisterEmailHandler : BaseHandler, IRequestHandler<ConfirmRegisterEmailCommand, ConfirmRegisterEmailResponse>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;

        public ConfirmRegisterEmailHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider, IEmailService emailService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.emailService = emailService;
        }
        public async Task<ConfirmRegisterEmailResponse> Handle(ConfirmRegisterEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await this.unitOfWork.Repository<UserInvitations>().FindAsync(s => s.UserEmail == request.Email);
            if (user == null)
            {
                throw new GeneralException(Constants.InvalidUserId);
            }

            if (user.EmailCodeExpiryDateTime < DateTime.UtcNow)
            {
                throw new GeneralException(Constants.EmailConfirmationCodeExpired);
            }

            if (!user.EmailConfirmationCode.Equals(request.Code))
            {
                throw new GeneralException(Constants.InvalidEmailConfirmationCode);
            }
            return new ConfirmRegisterEmailResponse()
            {
                UserEmail = user.UserEmail
            };
        }
    }
}
