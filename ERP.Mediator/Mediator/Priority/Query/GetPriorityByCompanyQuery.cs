using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Query
{
    public class GetPriorityByCompanyQuery : IRequest<List<GetPriority>>
    {
        public GetPriorityByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}