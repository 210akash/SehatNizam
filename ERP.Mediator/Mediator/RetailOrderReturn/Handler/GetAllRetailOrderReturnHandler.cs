using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class GetAllRetailOrderReturnHandler : IRequestHandler<GetAllRetailOrderReturnQuery, Tuple<IEnumerable<GetRetailOrderReturn>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRetailOrderReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRetailOrderReturn>, long>> Handle(GetAllRetailOrderReturnQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.RetailOrderReturn, bool>> predicate;
            Expression<Func<Entities.Models.RetailOrderReturn, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ModifiedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.RetailOrder,
                x => x.RetailOrderReturnDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new()
            {
                "RetailOrderReturnDetail.RetailOrderItems",
                "RetailOrderReturnDetail.RetailOrderItems.Item"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (string.IsNullOrEmpty(request.RetailOrderId) || x.RetailOrderId.ToString().Contains(request.RetailOrderId))
                      && x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (string.IsNullOrEmpty(request.RetailOrderId) || x.RetailOrderId.ToString().Contains(request.RetailOrderId))
                      && x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.RetailOrderReturn, object>> OrderBy = null;
            Expression<Func<Entities.Models.RetailOrderReturn, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var RetailOrderReturn = mapper.Map<IEnumerable<GetRetailOrderReturn>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRetailOrderReturn>, long>(RetailOrderReturn, entity.Item2);
        }
    }
}
