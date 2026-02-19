using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class GetIndentRequestByCompanyQuery : IRequest<List<GetIndentRequest>>
    {
        public GetIndentRequestByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}