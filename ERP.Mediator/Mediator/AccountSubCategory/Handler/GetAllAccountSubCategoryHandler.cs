using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class GetAllAccountSubCategoryHandler : IRequestHandler<GetAllAccountSubCategoryQuery, Tuple<IEnumerable<GetAccountSubCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountSubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccountSubCategory>, long>> Handle(GetAllAccountSubCategoryQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AccountSubCategory, bool>> predicate;

            Expression<Func<Entities.Models.AccountSubCategory, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.AccountCategory
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.AccountCategoryId == null || x.AccountCategoryId == request.AccountCategoryId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.AccountCategoryId == null || x.AccountCategoryId == request.AccountCategoryId);
            }

            Expression<Func<Entities.Models.AccountSubCategory, object>> OrderBy = null;
            Expression<Func<Entities.Models.AccountSubCategory, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AccountSubCategory = mapper.Map<IEnumerable<GetAccountSubCategory>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetAccountSubCategory>, long>(AccountSubCategory, entity.Item2);
        }
    }
}
