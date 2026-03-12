using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class GetAllRetailOrderHandler : IRequestHandler<GetAllRetailOrderQuery, Tuple<IEnumerable<GetRetailOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;

        public GetAllRetailOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRetailOrder>, long>> Handle(GetAllRetailOrderQuery request, CancellationToken cancellationToken)
        {
            string role = authService.GetCurrentUserRole();
            Expression<Func<Entities.Models.RetailOrder, bool>> predicate = null;

            predicate = x => x.IsActive == true
                && (x.ShopId == sessionProvider.Session.RetailUserShopId)
                && (request.RetailOrderId == null || x.Id == request.RetailOrderId)
                && (request.StatusId == 0 || x.RetailOrderStatusId == request.StatusId)
                && x.CreatedDate >= request.FDate
                && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1);

            Expression<Func<Entities.Models.RetailOrder, object>>[] includes = {
                x => x.Shop,
                x => x.CreatedBy,
                x => x.Shop.Territory.Area.Zone,
                x => x.Shop.Territory,
                x => x.RetailOrderStatus,
                x => x.RetailOrderProcess
            };

            Expression<Func<Entities.Models.RetailOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.RetailOrder, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("RetailOrderProcess.FromStatus");
            thenInclude.Add("RetailOrderProcess.ToStatus");

            var entity = unitOfWork.Repository<Entities.Models.RetailOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var RetailOrder = mapper.Map<IEnumerable<GetRetailOrder>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRetailOrder>, long>(RetailOrder, entity.Item2);
        }
    }
}
