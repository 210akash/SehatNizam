using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetDispatchByCompanyQuery : IRequest<List<GetDispatch>>
    {
        public GetDispatchByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}