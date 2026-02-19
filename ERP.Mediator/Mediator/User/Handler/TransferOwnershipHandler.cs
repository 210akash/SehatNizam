using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
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
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class TransferOwnershipHandler : BaseHandler, IRequestHandler<TransferOwnershipCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TransferOwnershipHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider,IEmailService emailService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.emailService = emailService;
        }
        public async Task<bool> Handle(TransferOwnershipCommand request, CancellationToken cancellationToken)
        {
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (this.SessionProvider.Session.RoleId.Contains(Guid.Parse(Constants.AccountOwnerRoleId)))
            {
                var transferToUser = new AspNetUserRoles();
                transferToUser = this.unitOfWork.Repository<AspNetUserRoles>().Find(u => u.UserId == request.TransferToUserId);
                if (transferToUser != null)
                {
                    //transferToUser.RoleId = Guid.Parse(Constants.AccountOwnerRoleId);
                    //this.unitOfWork.Repository<AspNetUserRoles>().Update(transferToUser);
                    await this.unitOfWork.ExecuteSqlCommandAsync("UPDATE AspNetUserRoles SET ROLEID = '" + Constants.AccountOwnerRoleId + "' WHERE UserId = '" + request.TransferToUserId + "'");

                }
                else
                {
                    await this.unitOfWork.ExecuteSqlCommandAsync("INSERT INTO AspNetUserRoles VALUES ('" + request.TransferToUserId + "','" + Constants.AccountOwnerRoleId + "'");
                }

                await this.unitOfWork.ExecuteSqlCommandAsync("UPDATE AspNetUserRoles SET ROLEID = '"+ request.NewRoleId + "' WHERE UserId = '" + this.SessionProvider.Session.LoggedInUserId + "'");


                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
