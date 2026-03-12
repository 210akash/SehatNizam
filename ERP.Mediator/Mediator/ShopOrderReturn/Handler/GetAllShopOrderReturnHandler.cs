using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class GetAllShopOrderReturnHandler : IRequestHandler<GetAllShopOrderReturnQuery, Tuple<IEnumerable<GetShopOrderReturn>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllShopOrderReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetShopOrderReturn>, long>> Handle(GetAllShopOrderReturnQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.ShopOrderReturn, bool>> predicate;
            Expression<Func<Entities.Models.ShopOrderReturn, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ModifiedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.ShopOrder,
                x => x.ShopOrderReturnDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new()
            {
                "ShopOrderReturnDetail.ShopOrderItems",
                "ShopOrderReturnDetail.ShopOrderItems.Item"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                     // && x.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                   && (request.ShopOrderId == "" || x.ShopOrderId.ToString().Contains(request.ShopOrderId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.ShopOrderId == "" || x.ShopOrderId.ToString().Contains(request.ShopOrderId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.ShopOrderReturn, object>> OrderBy = null;
            Expression<Func<Entities.Models.ShopOrderReturn, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var ShopOrderReturn = mapper.Map<IEnumerable<GetShopOrderReturn>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetShopOrderReturn>, long>(ShopOrderReturn, entity.Item2);
        }
    }
}
