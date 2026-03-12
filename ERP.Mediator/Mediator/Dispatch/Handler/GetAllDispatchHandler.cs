using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetAllDispatchHandler : IRequestHandler<GetAllDispatchQuery, Tuple<IEnumerable<GetDispatch>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllDispatchHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetDispatch>, long>> Handle(GetAllDispatchQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Dispatch, bool>> predicate;
            Expression<Func<Entities.Models.Dispatch, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.Vehicle,
                x => x.Project,
                x => x.DispatchOrder.Where(x => x.IsActive)
            };

            List<string> thenIncludes = new()
            {
                "DispatchOrder.DispatchDetail",
                "DispatchOrder.DispatchDetail.OrderItem",
                "DispatchOrder.DispatchDetail.OrderItem.Item",
                "DispatchOrder.DispatchDetail.OrderItem.Item.UOM",
                "DispatchOrder.Order",
                "DispatchOrder.Order.Dealership",
                "DispatchOrder.Order.Dealership.Territory",
                "DispatchOrder.DispatchDetail.CostSheet"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId == 0 || x.DispatchOrder.Any(y=>y.Order.DealershipId == request.DealershipId))
                      && (request.OrderId == "" || x.DispatchOrder.Any(y => y.Order.Id.ToString() == request.OrderId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId == 0 || x.DispatchOrder.Any(y => y.Order.DealershipId == request.DealershipId))
                      && (request.OrderId == "" || x.DispatchOrder.Any(y => y.Order.Id.ToString() == request.OrderId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId == 0 || x.DispatchOrder.Any(y => y.Order.DealershipId == request.DealershipId))
                      && (request.OrderId == "" || x.DispatchOrder.Any(y => y.Order.Id.ToString() == request.OrderId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.Dispatch, object>> OrderBy = null;
            Expression<Func<Entities.Models.Dispatch, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Dispatch>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Dispatch = mapper.Map<IEnumerable<GetDispatch>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetDispatch>, long>(Dispatch, entity.Item2);
        }
    }
}
