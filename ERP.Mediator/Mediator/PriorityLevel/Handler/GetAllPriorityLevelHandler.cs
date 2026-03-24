using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PriorityLevel.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PriorityLevel.Handler
{
    public class GetAllPriorityLevelHandler : IRequestHandler<GetAllPriorityLevelQuery, Tuple<IEnumerable<GetPriorityLevel>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPriorityLevelHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPriorityLevel>, long>> Handle(GetAllPriorityLevelQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PriorityLevel, bool>> predicate;

            Expression<Func<Entities.Models.PriorityLevel, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Doctor") || roles.Contains(""))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }
            else
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }

            Expression<Func<Entities.Models.PriorityLevel, object>> OrderBy = null;
            Expression<Func<Entities.Models.PriorityLevel, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.PriorityLevel>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var PriorityLevel = mapper.Map<IEnumerable<GetPriorityLevel>>(entity.Item1);

            return new Tuple<IEnumerable<GetPriorityLevel>, long>(PriorityLevel, entity.Item2);
        }
    }
}
