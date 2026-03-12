using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Services.Interfaces;
using ERP.Core.Provider;
using ERP.BusinessModels.Enums;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class GetAllOrderHandler : IRequestHandler<GetAllOrderQuery, Tuple<IEnumerable<GetOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;
        public GetAllOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetOrder>, long>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            string role = authService.GetCurrentUserRole();
            Expression<Func<Entities.Models.Order, bool>> predicate = null;

            if (role == "Distributor" || role == "ASE" || role == "DSF")
            {
                predicate = x => x.IsActive == true
                    //&& (request.StatusId == null || request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    //&& (request.FDate == null || request.TDate == null
                    //    || (x.CreatedDate >= request.FDate && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)))
                    && (request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)

                    && x.DealershipId == sessionProvider.Session.DealershipId

                    && (request.RegionId == 0 || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Dealership.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Dealership.TerritoryId == request.TerritoryId)

                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    ;
            }
            else if (role == "AM ACC")
            {
                predicate = x => x.IsActive == true
                    && x.OrderStatusId == (long)OrderStatusEnum.OrderCreate
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)

                    && x.DealershipId != 0 && x.DealershipId != null

                    && (request.RegionId == 0 || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Dealership.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Dealership.TerritoryId == request.TerritoryId)

                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    ;
            }
            else if (role == "MAN ACC")
            {
                predicate = x => x.IsActive == true
                    && x.OrderStatusId == (long)OrderStatusEnum.OrderConfirm
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)

                    && x.DealershipId != 0 && x.DealershipId != null

                    && (request.RegionId == 0 || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Dealership.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Dealership.TerritoryId == request.TerritoryId)

                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    ;
            }
            else if (role == "KSS")
            {
                predicate = x => x.IsActive == true
                    && (x.OrderStatusId == (long)OrderStatusEnum.OrderCreate || x.OrderStatusId == (long)OrderStatusEnum.OrderConfirm)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)

                    //&& x.DealershipId != 0 && x.DealershipId != null
                    && x.DealershipId == 7 //Head Office Dealership

                    && (request.RegionId == 0 || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Dealership.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Dealership.TerritoryId == request.TerritoryId)

                    //&& (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    ;
            }
            else
            {
                if(request.StatusId == 99)
                {
                    predicate = x => x.IsActive == true
                    && (x.OrderStatusId == 30 || x.OrderStatusId == 40)
                    && x.DealershipId != 0 && x.DealershipId != null
                    ;
                }
                else
                {
                    predicate = x => x.IsActive == true
                    //&& (request.StatusId == null || request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    //&& (request.FDate == null || request.TDate == null
                    //    || (x.CreatedDate >= request.FDate && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)))
                    && (request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                    && (string.IsNullOrWhiteSpace(request.OrderId.ToString())
                      || x.Id.ToString().Contains(request.OrderId.ToString().Trim()))
                    && x.DealershipId != 0 && x.DealershipId != null

                    && (request.RegionId == 0 || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Dealership.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Dealership.TerritoryId == request.TerritoryId)

                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId)
                    ;
                }
                
            }

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.OrderItems,
                x => x.Dealership,
                x => x.OrderStatus,
                x => x.OrderProcess,
                x => x.Dealership.Territory,
                x => x.Dealership.Territory.Area.Zone,
                x => x.CancelDispatch,
                x => x.CreatedBy.Department,
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");
            thenInclude.Add("OrderItems.Item");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var order = mapper.Map<IEnumerable<GetOrder>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetOrder>, long>(order, entity.Item2);
        }
    }
}
