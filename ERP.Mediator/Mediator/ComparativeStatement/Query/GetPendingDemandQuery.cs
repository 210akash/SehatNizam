using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class GetPendingDemandQuery : IRequest<List<GetDropDown>>
    {
        public GetPendingDemandQuery(long PurchaseDemandId, long ComparativeStatementId)
        {
            this.PurchaseDemandId = PurchaseDemandId;
            this.ComparativeStatementId = ComparativeStatementId;
        }

        public long PurchaseDemandId { get; set; }
        public long ComparativeStatementId { get; set; }
    }
}