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
using Microsoft.AspNetCore.Http;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetPendingIndentRequestHandler : IRequestHandler<GetPendingIndentRequestQuery, List<GetIndentRequest>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetPendingIndentRequestHandler(IHttpContextAccessor httpContextAccessor,IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetIndentRequest>> Handle(GetPendingIndentRequestQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.IndentRequest, object>> orderByExpression = x => x.ApprovedDate;

            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetAsync(
                x => x.IsActive &&
                x.StatusId == 3
               && x.ProjectId == sessionProvider.Session.SelectedWarehouseId,
                includeProperties: "IndentRequestDetail,Department,Store,Project",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var processedDetailIds = unitOfWork.Repository<IssuanceDetail>()
                .GetAll()
                .GroupBy(pd => pd.IndentRequestDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalOrderedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);

            List<Entities.Models.IndentRequest> pendingIndentRequest = new();

            foreach (var item in IndentRequest)
            {
                if (item.Id != request.IndentRequestId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.IndentRequestDetail.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.Required > totalOrderedQty)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingIndentRequest.Add(item);
                    }
                }
                else
                    pendingIndentRequest.Add(item);
            }

            return mapper.Map<List<GetIndentRequest>>(pendingIndentRequest);
        }
    }
}
