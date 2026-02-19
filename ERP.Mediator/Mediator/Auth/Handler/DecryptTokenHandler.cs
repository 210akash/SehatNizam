using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class DecryptTokenHandler : BaseHandler, IRequestHandler<DecryptTokenQuery, DecryptTokenModel>
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

        private readonly IAuthService authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public DecryptTokenHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IAuthService authService, SessionProvider sessionProvider) : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.authService = authService;
        }

        public async Task<DecryptTokenModel> Handle(DecryptTokenQuery request, CancellationToken cancellationToken)
        {
            //var token = this.authService.Decrypt(request.Token).Split(",");
            //var isCancleUser = this.unitOfWork.Repository<UserInvitations>().Find(u => u.UserEmail == token[0] && u.RoleId == Guid.Parse(token[1]) && u.CompanyId == Convert.ToInt64(token[2]));
            //if (isCancleUser.IsCancel == true)
            //{
            //    throw new Exception("It seems your invitation was cancelled by an administrator. Try asking again?");
            //}
            //var company = await this.unitOfWork.Repository<Entities.Models.Company>().FindAsync(c => c.CompanyId == Convert.ToInt32(token[2]));
            //var title = await this.GetRoleByIdAsync(token[1]);
            //if (company != null)
            //{
            //    var registerCommand = new DecryptTokenModel()
            //    {
            //        Email = token[0],
            //        RoleId = Guid.Parse(token[1]),
            //        CompanyId = company.CompanyId,
            //        CompanyName = company.CompanyName,
            //        IndustryId = company.IndustryId,
            //        Title = title.Name
            //    };
            //    return registerCommand;
            //}
            //else
            //{
            //    return null;
            //}
            return null;
                
        }
        private async Task<AspNetRoles> GetRoleByIdAsync(string roleName)
        {
            var role = await this.unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Id == Guid.Parse(roleName));
            return role;
        }
    }
}
