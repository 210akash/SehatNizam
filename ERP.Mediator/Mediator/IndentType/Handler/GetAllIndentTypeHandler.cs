using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class GetAllIndentTypeHandler : IRequestHandler<GetAllIndentTypeQuery, Tuple<IEnumerable<GetIndentType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllIndentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetIndentType>, long>> Handle(GetAllIndentTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.IndentType, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.IndentType, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }

            Expression<Func<Entities.Models.IndentType, object>> OrderBy = null;
            Expression<Func<Entities.Models.IndentType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.IndentType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var IndentType = mapper.Map<IEnumerable<GetIndentType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetIndentType>, long>(IndentType, entity.Item2);
        }
    }
}
