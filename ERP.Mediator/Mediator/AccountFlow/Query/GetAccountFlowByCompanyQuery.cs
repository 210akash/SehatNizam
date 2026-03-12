using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Query
{
    public class GetAccountFlowByCompanyQuery : IRequest<List<GetAccountFlow>>
    {
        public GetAccountFlowByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}