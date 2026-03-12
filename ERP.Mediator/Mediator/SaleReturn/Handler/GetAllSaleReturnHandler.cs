using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class GetAllSaleReturnHandler : IRequestHandler<GetAllSaleReturnQuery, Tuple<IEnumerable<GetSaleReturn>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSaleReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSaleReturn>, long>> Handle(GetAllSaleReturnQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SaleReturn, bool>> predicate;
            Expression<Func<Entities.Models.SaleReturn, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ModifiedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.DispatchOrder,
                x => x.Project,
                x => x.SaleReturnDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new()
            {
                "DispatchOrder.Order",
                "DispatchOrder.Order.Dealership",
                "SaleReturnDetail.DispatchDetail.OrderItem",
                "SaleReturnDetail.DispatchDetail.OrderItem.Item"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId ==  0 || x.DispatchOrder.Order.DealershipId == request.DealershipId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId == 0 || x.DispatchOrder.Order.DealershipId == request.DealershipId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.SaleReturn, object>> OrderBy = null;
            Expression<Func<Entities.Models.SaleReturn, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SaleReturn>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var SaleReturn = mapper.Map<IEnumerable<GetSaleReturn>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetSaleReturn>, long>(SaleReturn, entity.Item2);
        }
    }
}
