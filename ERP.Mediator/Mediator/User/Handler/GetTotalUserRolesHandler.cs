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
    public class GetTotalUserRolesHandler : BaseHandler, IRequestHandler<GetTotalUserRolesQuery, List<GetTotalUserRolesResponse>>
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
        public GetTotalUserRolesHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<List<GetTotalUserRolesResponse>> Handle(GetTotalUserRolesQuery request, CancellationToken cancellationToken)
        {
            var query = string.Format("spGetTotalUserRoles @CompanyId={0}", this.SessionProvider.Session.CompanyId);
            var result = await this.unitOfWork.Repository<spGetTotalUserRoles>().FromSqlRaw(query);
            return this.mapper.Map<List<GetTotalUserRolesResponse>>(result);
        }
    }

}
