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
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetPendingIndentRequestItemsHandler : IRequestHandler<GetPendingIndentRequestItemsQuery, List<GetIndentRequestDetail>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingIndentRequestItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetIndentRequestDetail>> Handle(GetPendingIndentRequestItemsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<IndentRequestDetail, object>> orderByExpression = x => x.Id;
            Expression<Func<IndentRequestDetail, object>> includeProperties = x => x.Item.UOM;

            Expression<Func<IndentRequestDetail, object>>[] includes = {
                x => x.Item,
                x => x.Item.UOM,
             };

            Expression<Func<IndentRequestDetail, bool>> predicate = x => x.IsActive == true
                   && x.IndentRequestId == request.IndentRequestId;

            var entity = unitOfWork.Repository<IndentRequestDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var IndentRequest = entity.Item1.ToList();

            var processedDetailIds = unitOfWork.Repository<IssuanceDetail>()
                .FindAll(x => x.IsActive == true && x.IndentRequestDetail.IndentRequestId == request.IndentRequestId)
                .GroupBy(pd => pd.IndentRequestDetailId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<IndentRequestDetail> pendingIndentRequests = new();

            if (request.IssuanceId == 0)
            {
                pendingIndentRequests = IndentRequest
                   .Where(detail =>
                   {
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                       return detail.Required > totalOrderedQty;
                   })
                   .ToList();

                foreach (var detail in pendingIndentRequests)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Required -= totalOrderedQty;
                }
            }
            else
            {
                pendingIndentRequests = IndentRequest;
            }

            foreach (var item in pendingIndentRequests)
            {

            }

            var IndentRequestDetail = mapper.Map<IEnumerable<GetIndentRequestDetail>>(pendingIndentRequests).ToList();
            return IndentRequestDetail;
        }
    }
}
