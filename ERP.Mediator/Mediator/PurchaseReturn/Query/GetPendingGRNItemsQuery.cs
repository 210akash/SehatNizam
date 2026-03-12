using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class GetPendingGRNItemsQuery : IRequest<List<GetGRNDetail>>
    {
        public GetPendingGRNItemsQuery(long GRNId, long PurchaseReturnId)
        {
            this.GRNId = GRNId;
            this.PurchaseReturnId = PurchaseReturnId;
        }

        public long GRNId { get; set; }
        public long PurchaseReturnId { get; set; }
    }
}