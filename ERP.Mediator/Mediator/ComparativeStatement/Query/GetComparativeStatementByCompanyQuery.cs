using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class GetComparativeStatementByCompanyQuery : IRequest<List<GetComparativeStatement>>
    {
        public GetComparativeStatementByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}