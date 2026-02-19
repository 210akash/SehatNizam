using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class GetAllAccountCategoryHandler : IRequestHandler<GetAllAccountCategoryQuery, Tuple<IEnumerable<GetAccountCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccountCategory>, long>> Handle(GetAllAccountCategoryQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AccountCategory, bool>> predicate;

            Expression<Func<Entities.Models.AccountCategory, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.AccountHead
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true
                &&(request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()))
                && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));
            }

            Expression<Func<Entities.Models.AccountCategory, object>> OrderBy = null;
            Expression<Func<Entities.Models.AccountCategory, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AccountCategory>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AccountCategory = mapper.Map<IEnumerable<GetAccountCategory>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetAccountCategory>, long>(AccountCategory, entity.Item2);
        }
    }
}
