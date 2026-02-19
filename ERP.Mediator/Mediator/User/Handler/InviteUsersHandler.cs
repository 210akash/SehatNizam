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
    public class InviteUsersHandler : BaseHandler, IRequestHandler<InviteUsersCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;
        private readonly IAuthService authService;

        public InviteUsersHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider, IEmailService emailService, IAuthService authService)
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
        public async Task<bool> Handle(InviteUsersCommand request, CancellationToken cancellationToken)
        {
            List<UserInvitations> userInvitationsList = new List<UserInvitations>();
            foreach (var req in request.inviteUsers)
            {
                var InvitedUser = await this.unitOfWork.Repository<UserInvitations>().FindAsync(o => o.UserEmail == req.UserEmail);
                var aspUser = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(u => u.Email == req.UserEmail);
                if (InvitedUser == null && aspUser == null)
                {
                    var model = new UserInvitations()
                    {
                        UserEmail = req.UserEmail,
                        RoleId = req.RoleId,
                        IsPending = true,
                        DateCreated = DateTime.UtcNow,
                        ReminderDate = DateTime.UtcNow,
                        CreatedBy = this.SessionProvider.Session.LoggedInUserId,
                        CompanyId = this.SessionProvider.Session.CompanyId
                    };

                    userInvitationsList.Add(model);
                }

                var baseUrl = string.Format("{0}/auth/register?token=", this.config["WebApplication:LiveUrl"]);
                var user = await this.unitOfWork.Repository<AspNetUsers>().FindAsync(u => u.Id == this.SessionProvider.Session.LoggedInUserId);
                //// Send Email To Users Here
                var encryptLink = this.authService.Encrypt($"{req.UserEmail},{req.RoleId},{SessionProvider.Session.CompanyId}");
                await this.emailService.SendAsync(req.UserEmail,
                Constants.UserInvitationSubject,
                $"{user.FirstName} {user.LastName}",
                new List<EmailModelButtons>() { new EmailModelButtons() { BtnName = "Join Sensyrtech", BtnLink = string.Concat(baseUrl, encryptLink) } },

                Constants.InvitationEmailTemplate,
                string.Empty);
            }
            await this.unitOfWork.Repository<UserInvitations>().AddRangeAsync(userInvitationsList);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}
