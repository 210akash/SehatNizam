using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.VisitType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Handler
{
    public class GetAllVisitTypeHandler : IRequestHandler<GetAllVisitTypeQuery, Tuple<IEnumerable<GetVisitType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllVisitTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetVisitType>, long>> Handle(GetAllVisitTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.VisitType, bool>> predicate;

            Expression<Func<Entities.Models.VisitType, object>>[] includes = {
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

            Expression<Func<Entities.Models.VisitType, object>> OrderBy = null;
            Expression<Func<Entities.Models.VisitType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.VisitType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var VisitType = mapper.Map<IEnumerable<GetVisitType>>(entity.Item1);

            return new Tuple<IEnumerable<GetVisitType>, long>(VisitType, entity.Item2);
        }
    }
}
