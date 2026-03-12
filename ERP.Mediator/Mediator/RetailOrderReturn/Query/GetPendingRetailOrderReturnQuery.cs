using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Query
{
    public class GetPendingRetailOrderQuery : IRequest<List<GetRetailOrder>>
    {
        public GetPendingRetailOrderQuery(long RetailOrderId, string searchParam)
        {
            this.RetailOrderId = RetailOrderId;
            this.searchParam = searchParam; 
        }

        public long RetailOrderId { get; set; }
        public string searchParam { get; set; }
    }
}