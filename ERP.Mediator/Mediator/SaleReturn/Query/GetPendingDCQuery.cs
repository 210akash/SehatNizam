using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class GetPendingDCQuery : IRequest<List<GetDispatchOrder>>
    {
        public GetPendingDCQuery(long DispatchedOrderId, string searchParam)
        {
            this.DispatchedOrderId = DispatchedOrderId;
            this.searchParam = searchParam; 
        }

        public long DispatchedOrderId { get; set; }
        public string searchParam { get; set; }
    }
}