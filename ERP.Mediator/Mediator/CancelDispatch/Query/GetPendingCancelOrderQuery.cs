using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.CancelDispatch.Query
{
    public class GetPendingCancelOrderQuery : IRequest<List<GetOrder>>
    {
        public GetPendingCancelOrderQuery(long CancelDispatchId)
        {
            this.CancelDispatchId = CancelDispatchId;
        }

        public long CancelDispatchId { get; set; }
    }
}
