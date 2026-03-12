using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.User.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class SendInvitationReminderHandler : BaseHandler, IRequestHandler<SendInvitationReminderCommand, DateTime>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;
        private readonly IAuthService authService;


        public SendInvitationReminderHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider, IEmailService emailService, IAuthService authService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.emailService = emailService;
            this.authService = authService;
        }
        public async Task<DateTime> Handle(SendInvitationReminderCommand request, CancellationToken cancellationToken)
        {
            var InvitedUser = await this.unitOfWork.Repository<UserInvitations>().GetFirstAsync(o => o.Id == request.Id);
            var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(u => u.Id == this.SessionProvider.Session.LoggedInUserId);
            var baseUrl = string.Format("{0}/auth/register?token=", this.config["WebApplication:LiveUrl"]);

            // Send Reminder
            try
            {
                var encryptLink = this.authService.Encrypt($"{InvitedUser.UserEmail},{InvitedUser.RoleId},{InvitedUser.CompanyId}");
                await this.emailService.SendAsync(InvitedUser.UserEmail,
                 Constants.InvitationReminderSubject,
                 $"{user.FirstName} {user.LastName}",
                 new List<EmailModelButtons>() { new EmailModelButtons() { BtnName = "Join Sensyrtech", BtnLink = string.Concat(baseUrl, encryptLink) } },
                 Constants.InvitationReminderTemplate,
                 string.Empty
                 );
                InvitedUser.ReminderDate = DateTime.UtcNow;
                this.unitOfWork.Repository<UserInvitations>().Update(InvitedUser);
                await this.unitOfWork.SaveChangesAsync();
                return (DateTime)InvitedUser.ReminderDate;
            }
            catch (Exception)
            {
                throw new Exception("Failed!");
            }
        }
    }
}
