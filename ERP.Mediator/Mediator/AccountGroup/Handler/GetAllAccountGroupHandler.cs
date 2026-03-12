using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Handler
{
    public class GetAllAccountGroupHandler : IRequestHandler<GetAllAccountGroupQuery, Tuple<IEnumerable<GetAccountGroup>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccountGroup>, long>> Handle(GetAllAccountGroupQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AccountGroup, bool>> predicate;

            Expression<Func<Entities.Models.AccountGroup, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.Account,
                x => x.Vendor,
                x => x.Dealership,
                x => x.Account.AccountFlow
            };

            // Check if the current user's RoleId array contains the AccountGroupOwnerRoleId
            if (roles.Contains("Account Manager") || roles.Contains("Account Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()) || x.Code.Contains(request.Name))
                && (request.AccountId == null || x.AccountId == request.AccountId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()) || x.Code.Contains(request.Name))
                && (request.AccountId == null || x.AccountId == request.AccountId);
            }

            Expression<Func<Entities.Models.AccountGroup, object>> OrderBy = null;
            Expression<Func<Entities.Models.AccountGroup, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AccountGroup>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AccountGroup = mapper.Map<IEnumerable<GetAccountGroup>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetAccountGroup>, long>(AccountGroup, entity.Item2);
        }
    }
}
