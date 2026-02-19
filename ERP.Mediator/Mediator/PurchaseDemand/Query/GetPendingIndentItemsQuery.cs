using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class GetPendingIndentItemsQuery : IRequest<List<GetIndentRequestDetail>>
    {
        public GetPendingIndentItemsQuery(long IndentRequestId, long PurchaseDemandId)
        {
            this.IndentRequestId = IndentRequestId;
            this.PurchaseDemandId = PurchaseDemandId;
        }

        public long IndentRequestId { get; set; }
        public long PurchaseDemandId { get; set; }
    }
}