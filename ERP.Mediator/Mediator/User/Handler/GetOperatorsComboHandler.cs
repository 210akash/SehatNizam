//-----------------------------------------------------------------------
// <copyright file="RegisterHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    public class GetOperatorsComboHandler : BaseHandler, IRequestHandler<GetOperatorsComboQuery, List<AspNetUsersModel>>
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
        public GetOperatorsComboHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<List<AspNetUsersModel>> Handle(GetOperatorsComboQuery request, CancellationToken cancellationToken)
        {
            var operators = this.unitOfWork.Repository<AspNetUsers>().Set
                //.Where(w => (string.IsNullOrEmpty(request.Search) || w.FirstName.Contains(request.Search) || w.LastName.Contains(request.Search) || w.Email.Contains(request.Search)) && w.CompanyId == this.SessionProvider.Session.CompanyId)
                .Join(this.unitOfWork.Repository<AspNetUserRoles>().Set,
                u => u.Id,
                r => r.UserId,
                (aspUser, role) => new AspNetUsersModel
                {
                    Id = aspUser.Id,
                    FirstName = aspUser.FirstName,
                    LastName = aspUser.LastName,
                    Email = aspUser.NormalizedEmail,
                    Title = aspUser.Title,
                    CreatedDate = aspUser.CreatedDate,
                    PhoneNumber = aspUser.PhoneNumber,
                    ProfileBlobUrl = aspUser.ProfileBlobUrl,
                    //CompanyId = aspUser.CompanyId
                   // RoleId = role.RoleId
                })
               // .Where(o => o.RoleId == Guid.Parse(Constants.OperatorRoleId))
                .ToList();

            return operators;
        }
    }
}