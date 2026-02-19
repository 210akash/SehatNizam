using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class GetPendingIndentRequestQuery : IRequest<List<GetDropDown>>
    {
        public GetPendingIndentRequestQuery(long IndentRequestId)
        {
            this.IndentRequestId = IndentRequestId;
        }

        public long IndentRequestId { get; set; }
    }
}