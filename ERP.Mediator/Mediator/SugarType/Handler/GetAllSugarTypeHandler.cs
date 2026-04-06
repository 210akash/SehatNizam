using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SugarType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Handler
{
    public class GetAllSugarTypeHandler : IRequestHandler<GetAllSugarTypeQuery, Tuple<IEnumerable<GetSugarType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSugarTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSugarType>, long>> Handle(GetAllSugarTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SugarType, bool>> predicate;

            Expression<Func<Entities.Models.SugarType, object>>[] includes = {
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

            Expression<Func<Entities.Models.SugarType, object>> OrderBy = null;
            Expression<Func<Entities.Models.SugarType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SugarType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var SugarType = mapper.Map<IEnumerable<GetSugarType>>(entity.Item1);

            return new Tuple<IEnumerable<GetSugarType>, long>(SugarType, entity.Item2);
        }
    }
}
