using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.App.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.App.Handler
{
    public class GetDealershipOrderHandler : IRequestHandler<GetDealershipOrderQuery, Tuple<IEnumerable<GetDealershipOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetDealershipOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetDealershipOrder>, long>> Handle(GetDealershipOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Order, bool>> predicate = null;

                predicate = x => x.IsActive == true
                    && (request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                    && x.DealershipId == request.DealershipId
                    && (request.DealershipId == 0 || x.DealershipId == request.DealershipId);

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.OrderItems,
                x => x.Dealership,
                x => x.OrderStatus,
                x => x.OrderProcess,
                x => x.OrderAttachments,
                x => x.CancelDispatch,
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");
            thenInclude.Add("OrderItems.Item");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var order = mapper.Map<IEnumerable<GetDealershipOrder>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetDealershipOrder>, long>(order, entity.Item2);
        }
    }
}
