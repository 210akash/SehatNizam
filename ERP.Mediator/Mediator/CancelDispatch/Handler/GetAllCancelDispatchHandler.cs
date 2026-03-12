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
    public class GetAllCancelDispatchHandler : IRequestHandler<GetAllCancelDispatchQuery, Tuple<IEnumerable<GetCancelDispatch>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllCancelDispatchHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetCancelDispatch>, long>> Handle(GetAllCancelDispatchQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.CancelDispatch, bool>> predicate;

            predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));

            Expression<Func<Entities.Models.CancelDispatch, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.Order,
                x => x.Order.Dealership,
                x => x.OrderProcess,
                x => x.CancelDispatchDetail.Where(x => x.IsActive)
            };

            List<string> thenIncludes = new();
            thenIncludes.Add("CancelDispatchDetail.OrderItem");
            thenIncludes.Add("CancelDispatchDetail.OrderItem.Item");
            thenIncludes.Add("OrderProcess.FromStatus");
            thenIncludes.Add("OrderProcess.ToStatus");
            thenIncludes.Add("OrderProcess.CreatedBy");

            Expression<Func<Entities.Models.CancelDispatch, object>> OrderBy = null;
            Expression<Func<Entities.Models.CancelDispatch, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.CancelDispatch>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var CancelDispatch = mapper.Map<IEnumerable<GetCancelDispatch>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetCancelDispatch>, long>(CancelDispatch, entity.Item2);
        }
    }
}
