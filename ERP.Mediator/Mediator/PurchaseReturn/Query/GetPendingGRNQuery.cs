using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class GetPendingGRNQuery : IRequest<List<GetGRN>>
    {
        public GetPendingGRNQuery(long GRNId, string searchParam)
        {
            this.GRNId = GRNId;
            this.searchParam = searchParam; 
        }

        public long GRNId { get; set; }
        public string searchParam { get; set; }
    }
}