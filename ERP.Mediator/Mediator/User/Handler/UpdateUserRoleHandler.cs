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
    public class UpdateUserRoleHandler : BaseHandler, IRequestHandler<UpdateUserRoleCommand, bool>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IEmailService emailService;

        public UpdateUserRoleHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider, IEmailService emailService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.emailService = emailService;
        }
        public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            // Due to AspNet Identity Restriction in C#. We First have to remove and then add the role in order to Update
            // Removing
            var userRole = await this.unitOfWork.Repository<AspNetUserRoles>().GetFirstAsync(u => u.UserId == request.UserId);
            this.unitOfWork.Repository<AspNetUserRoles>().Remove(userRole);
            await this.unitOfWork.SaveChangesAsync();

            // Creating New
            AspNetUserRoles newRole = new AspNetUserRoles();
            newRole.UserId = request.UserId;
            newRole.RoleId = request.RoleId;

            await this.unitOfWork.Repository<AspNetUserRoles>().AddAsync(newRole);
            await this.unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
