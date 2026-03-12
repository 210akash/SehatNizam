using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class GetIssuanceByCompanyQuery : IRequest<List<GetIssuance>>
    {
        public GetIssuanceByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}