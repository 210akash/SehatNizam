using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class GetPendingIndentRequestQuery : IRequest<List<GetIndentRequest>>
    {
        public GetPendingIndentRequestQuery(long IndentRequestId)
        {
            this.IndentRequestId = IndentRequestId;
        }

        public long IndentRequestId { get; set; }
    }
}