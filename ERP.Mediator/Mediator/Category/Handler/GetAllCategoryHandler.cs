using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Category.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Handler
{
    public class GetAllCategoryHandler : IRequestHandler<GetAllCategoryQuery, Tuple<IEnumerable<GetCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetCategory>, long>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Category, bool>> predicate = x => x.IsActive == true 
           ;

            Expression<Func<Entities.Models.Category, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.CategoryStores
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true 
                && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }

            Expression<Func<Entities.Models.Category, object>> OrderBy = null;
            Expression<Func<Entities.Models.Category, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Category>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Category = mapper.Map<IEnumerable<GetCategory>>(entity.Item1.ToList()).ToList();

            foreach (var item in Category)
            {
                List<long> storeIds = new List<long>();
                foreach (var role in item.CategoryStores.Where(y=>y.IsActive = true))
                {
                    storeIds.Add(role.StoreId);
                }
                item.StoreIds = storeIds;
            }

            return new Tuple<IEnumerable<GetCategory>, long>(Category, entity.Item2);
        }
    }
}
