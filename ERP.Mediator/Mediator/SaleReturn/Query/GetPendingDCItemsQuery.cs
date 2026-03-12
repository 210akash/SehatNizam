using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class GetPendingDCItemsQuery : IRequest<List<GetDispatchDetail>>
    {
        public GetPendingDCItemsQuery(long DispatchOrderId, long SaleReturnId)
        {
            this.DispatchOrderId = DispatchOrderId;
            this.SaleReturnId = SaleReturnId;
        }

        public long DispatchOrderId { get; set; }
        public long SaleReturnId { get; set; }
    }
}