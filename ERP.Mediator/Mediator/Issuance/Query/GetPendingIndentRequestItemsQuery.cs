using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class GetPendingIndentRequestItemsQuery : IRequest<List<GetIndentRequestDetail>>
    {
        public GetPendingIndentRequestItemsQuery(long IndentRequestId, long IssuanceId)
        {
            this.IndentRequestId = IndentRequestId;
            this.IssuanceId = IssuanceId;
        }

        public long IndentRequestId { get; set; }
        public long IssuanceId { get; set; }
    }
}