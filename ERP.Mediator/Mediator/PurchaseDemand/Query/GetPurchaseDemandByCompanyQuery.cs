using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class GetPurchaseDemandByCompanyQuery : IRequest<List<GetPurchaseDemand>>
    {
        public GetPurchaseDemandByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}