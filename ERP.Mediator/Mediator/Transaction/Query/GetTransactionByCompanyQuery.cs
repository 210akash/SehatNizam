using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetTransactionByCompanyQuery : IRequest<List<GetTransaction>>
    {
        public GetTransactionByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}