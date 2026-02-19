//-----------------------------------------------------------------------
// <copyright file="RegisterHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.Auth.Validator;
using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// Check Password Handler
    /// </summary>
    /// <seealso cref="TransferMaster.Mediator.Mediator.BaseHandler" />
    /// <seealso cref="IRequestHandler{TransferMaster.Mediator.Mediator.Auth.Command.LoginCommand, TransferMaster.BusinessModels.ResponseVM.TokenVM}" />
    public class RegisterHandler : BaseHandler, IRequestHandler<RegisterCommand, IdentityResponse>
    {
        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;
        private readonly RegisterValidator validatior;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public RegisterHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, RegisterValidator validatior, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validatior = validatior;
            this.userManager = userManager;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<IdentityResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            validatior.ValidateAndThrow(request);
            var result = new IdentityResponse();

            var user = new AspNetUsersModel
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Email.ToUpper(),
                EmailConfirmed = true,
                NormalizedUserName = request.Email,
                NormalizedEmail = request.Email,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                Title = request.Title,
                CreatedDate = DateTime.UtcNow,
                DepartmentId = request.DepartmentId,
                StoreId = request.StoreId,
                IsActive = true

            };
            var savedUser = await userManager.CreateAsync(user, request.Password);
            result = mapper.Map<IdentityResponse>(savedUser);
            if (result.Succeeded)
            {
                var getRole = await this.GetRoleByNameAsync(RolesPrefix.Agent);
                //string json = "{\"RoleId\": []}";
                //var data = JsonConvert.DeserializeObject<RegisterCommand>(json);
                foreach (var item in request.RoleId)
                {
                    var userRole = new AspNetUserRoles()
                    {
                        RoleId = new Guid(item != null ? item : item),
                        UserId = user.Id
                    };

                    await SaveAspNetUserRolesAsync(userRole);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the role by.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>the asp net roles</returns>
        private async Task<AspNetRoles> GetRoleByNameAsync(string roleName)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Name == roleName);
            return role;
        }

        /// <summary>
        /// Saves the ASP net user roles.
        /// </summary>
        /// <param name="model">The Asp Net User Roles model.</param>
        /// <returns>the task</returns>
        private async Task SaveAspNetUserRolesAsync(AspNetUserRoles model)
        {
            await unitOfWork.Repository<AspNetUserRoles>().AddAsync(model);
            await unitOfWork.SaveChangesAsync();
        }
    }
}