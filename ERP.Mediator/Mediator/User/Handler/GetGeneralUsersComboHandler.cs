using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.ComplexTypes;
using ERP.Entities.Models;
using ERP.Mediator.Mediator;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.User.Query;
using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class GetGeneralUsersComboHandler : BaseHandler, IRequestHandler<GetGeneralUsersComboQuery, List<GetGeneralUserResponse>>
    {
        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

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
        public GetGeneralUsersComboHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<List<GetGeneralUserResponse>> Handle(GetGeneralUsersComboQuery request, CancellationToken cancellationToken)
        {
            var genralUser = this.unitOfWork.Repository<AspNetUsers>().Set
                //.Where(u => u.CompanyId == this.SessionProvider.Session.CompanyId)
                .Join(this.unitOfWork.Repository<AspNetUserRoles>().Set
                .Where(r => r.RoleId != this.SessionProvider.Session.LoggedInUserId && r.RoleId != Guid.Parse(Constants.AccountOwnerRoleId)),
                u => u.Id,
                r => r.UserId,
                (aspUser, role) => new GetGeneralUserResponse
                {
                    Id = aspUser.Id,
                    FirstName = aspUser.FirstName,
                    LastName = aspUser.LastName,
                    RoleName = this.unitOfWork.Repository<AspNetRoles>().Set.Where(s => s.Id == role.RoleId).Select(n => n.Name).Single()
                }).ToList();

            return genralUser;
        }
    }

}
