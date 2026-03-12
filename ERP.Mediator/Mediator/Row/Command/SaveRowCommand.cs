using ERP.Mediator.Mediator.Rack.Command;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Row.Command
{
    public class SaveRowCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long RackId { get; set; }
    }
}
