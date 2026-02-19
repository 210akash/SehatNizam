using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.Auth.Validator;
using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class UpdateHandler : BaseHandler, IRequestHandler<UpdateCommand, IdentityResponse>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UpdateValidator validatior;
        private readonly UserManager<AspNetUsersModel> userManager;

        public UpdateHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, UpdateValidator validatior, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validatior = validatior;
            this.userManager = userManager;
        }

        public async Task<IdentityResponse> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            validatior.ValidateAndThrow(request);
            var result = new IdentityResponse();

            var user = await userManager.FindByEmailAsync(request.Email);
            user.Email = request.Email;
            user.UserName = request.Username;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Title = request.Title;
            user.IsActive = request.IsActive;
            var updateUser = await userManager.UpdateAsync(user);
            result = mapper.Map<IdentityResponse>(updateUser);

            if (result.Succeeded)
            {
                var userData = await unitOfWork.Repository<global::ERP.Entities.Models.AspNetUsers>().GetFirstAsync(y => y.Id == request.Id, null, null, "AspNetUserRoles");
                await RemoveExisitingRoles(request.Id);

                foreach (var item in request.RoleId)
                {
                    var userRole = new AspNetUserRoles()
                    {
                        RoleId = item,
                        UserId = user.Id
                    };

                    await SaveAspNetUserRolesAsync(userRole);
                }

                userData.Title = request.Title;
                userData.DepartmentId = request.DepartmentId;
                userData.StoreId = request.StoreId;
                userData.IsActive = request.IsActive;
                unitOfWork.Repository<AspNetUsers>().Update(userData);
                await unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        private async Task<AspNetRoles> GetRoleByNameAsync(string roleName)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Name == roleName);
            return role;
        }

        private async Task SaveAspNetUserRolesAsync(AspNetUserRoles model)
        {
            await unitOfWork.Repository<AspNetUserRoles>().AddAsync(model);
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<IEnumerable<AspNetUserRoles>> RemoveExisitingRoles(Guid? Id)
        {
            var role = await this.unitOfWork.Repository<AspNetUserRoles>().GetAsync(x => x.UserId == Id);
            foreach (var i in role)
            {
                unitOfWork.Repository<AspNetUserRoles>().Remove(i);
                await this.unitOfWork.SaveChangesAsync();
            }
            return role;
        }


    }
}