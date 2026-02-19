using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
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
    public class RegisterWithEmailHandler : BaseHandler, IRequestHandler<RegisterWithEmailCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;

        public RegisterWithEmailHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider, IEmailService emailService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.emailService = emailService;
        }
        public async Task<bool> Handle(RegisterWithEmailCommand request, CancellationToken cancellationToken)
        {
            UserInvitations model = null;
            var query = string.Format("spRegisterWithEmail @Email='{0}'", request.Email);
            var result = (await this.unitOfWork.Repository<spRegisterWithEmail>().FromSqlRaw(query)).FirstOrDefault();
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new GeneralException(result.ErrorMessage);
            }
            model = await this.unitOfWork.Repository<UserInvitations>().GetFirstAsync(o => o.UserEmail == request.Email);
            if (model == null)
            {
                model = new UserInvitations()
                {
                    EmailConfirmationCode = this.GenerateRandomSixDigitCode(),
                    EmailCodeExpiryDateTime = DateTime.UtcNow.AddMinutes(30),
                    UserEmail = request.Email,
                    RoleId = result.RoleId.Value,
                    IsPending = true,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = this.SessionProvider.Session.LoggedInUserId,
                    CompanyId = result.CompanyId?? 0
                };
            }
            else
            {
                model.EmailConfirmationCode = this.GenerateRandomSixDigitCode();
                model.EmailCodeExpiryDateTime = DateTime.UtcNow.AddMinutes(30);
            }
            await this.emailService.SendAsync(request.Email,
              Constants.EmailConfirmation, 
              model.EmailConfirmationCode,
              new List<EmailModelButtons>(),
              Constants.EmailConfirmationTemplate,
              string.Empty
              );
            if (model.Id == Guid.Empty)
                await this.unitOfWork.Repository<UserInvitations>().AddAsync(model);
            else
                this.unitOfWork.Repository<UserInvitations>().Update(model);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }
        private string GenerateRandomSixDigitCode()
        {
            var generator = new Random();
            string r = generator.Next(0, 999999).ToString("D6");
            return r;
        }
    }
}
