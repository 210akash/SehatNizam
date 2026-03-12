using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Query
{
    public class GetIndentTypeByCompanyQuery : IRequest<List<GetIndentType>>
    {
        public GetIndentTypeByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}