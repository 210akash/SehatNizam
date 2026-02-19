using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Query
{
    public class GetAccountHeadByCompanyQuery : IRequest<List<GetAccountHead>>
    {
        public GetAccountHeadByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}