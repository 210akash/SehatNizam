using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetAllItemHandler : IRequestHandler<GetAllItemQuery, Tuple<IEnumerable<GetItem>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllItemHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetItem>, long>> Handle(GetAllItemQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Item, bool>> predicate;

            Expression<Func<Entities.Models.Item, object>>[] includes = {
                x => x.CreatedBy,
                x => x.UOM,
                x => x.Company,
                x => x.ItemType,
                x => x.ItemType.SubCategory.Category
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.ItemTypeId == null || x.ItemTypeId == request.ItemTypeId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.ItemTypeId == null || x.ItemTypeId == request.ItemTypeId);
            }

            Expression<Func<Entities.Models.Item, object>> OrderBy = null;
            Expression<Func<Entities.Models.Item, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Item>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Item = mapper.Map<IEnumerable<GetItem>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetItem>, long>(Item, entity.Item2);
        }
    }
}
