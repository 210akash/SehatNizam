using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class GetAllItemTypeHandler : IRequestHandler<GetAllItemTypeQuery, Tuple<IEnumerable<GetItemType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllItemTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetItemType>, long>> Handle(GetAllItemTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.ItemType, bool>> predicate;

            Expression<Func<Entities.Models.ItemType, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.SubCategory,
                x => x.SubCategory.Category
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.SubCategoryId == null || x.SubCategoryId == request.SubCategoryId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.SubCategoryId == null || x.SubCategoryId == request.SubCategoryId);
            }

            Expression<Func<Entities.Models.ItemType, object>> OrderBy = null;
            Expression<Func<Entities.Models.ItemType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.ItemType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var ItemType = mapper.Map<IEnumerable<GetItemType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetItemType>, long>(ItemType, entity.Item2);
        }
    }
}
