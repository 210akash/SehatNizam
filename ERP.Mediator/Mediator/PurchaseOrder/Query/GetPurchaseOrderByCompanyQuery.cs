using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class GetPurchaseOrderByCompanyQuery : IRequest<List<GetPurchaseOrder>>
    {
        public GetPurchaseOrderByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}