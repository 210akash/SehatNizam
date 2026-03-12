using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dispatch.Command
{
    public class UpdateDispatchPrintStatusCommand : IRequest<long>
    {
        public long DispatchOrderId { get; set; }
    }
}
