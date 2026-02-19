using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Handler
{
    public class GetAllAccountTypeHandler : IRequestHandler<GetAllAccountTypeQuery, Tuple<IEnumerable<GetAccountType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccountType>, long>> Handle(GetAllAccountTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AccountType, bool>> predicate;

            Expression<Func<Entities.Models.AccountType, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.AccountSubCategory,
                x => x.AccountSubCategory.AccountCategory
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.AccountSubCategoryId == null || x.AccountSubCategoryId == request.AccountSubCategoryId);
            }
            else
            {
                predicate = x => x.IsActive == true
                 && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.AccountSubCategoryId == null || x.AccountSubCategoryId == request.AccountSubCategoryId);
            }

            Expression<Func<Entities.Models.AccountType, object>> OrderBy = null;
            Expression<Func<Entities.Models.AccountType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AccountType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AccountType = mapper.Map<IEnumerable<GetAccountType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetAccountType>, long>(AccountType, entity.Item2);
        }
    }
}
