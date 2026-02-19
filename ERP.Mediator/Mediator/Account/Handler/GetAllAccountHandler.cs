using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetAllAccountHandler : IRequestHandler<GetAllAccountQuery, Tuple<IEnumerable<GetAccount>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccount>, long>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Account, bool>> predicate;

            Expression<Func<Entities.Models.Account, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.AccountType,
                x => x.AccountFlow,
                x => x.AccountType.AccountSubCategory.AccountCategory
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()) || x.Code.Contains(request.Name))
                && (request.AccountTypeId == null || x.AccountTypeId == request.AccountTypeId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()) || x.Code.Contains(request.Name))
                && (request.AccountTypeId == null || x.AccountTypeId == request.AccountTypeId);
            }

            Expression<Func<Entities.Models.Account, object>> OrderBy = null;
            Expression<Func<Entities.Models.Account, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Account>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Account = mapper.Map<IEnumerable<GetAccount>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetAccount>, long>(Account, entity.Item2);
        }
    }
}
