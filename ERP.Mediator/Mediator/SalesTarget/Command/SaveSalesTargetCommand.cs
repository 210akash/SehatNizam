using MediatR;
using System;

namespace ERP.Mediator.Mediator.SalesTarget.Command
{
    public class SaveSalesTargetCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long Target { get; set; }
        public DateTime TargetMonth { get; set; }
        public long? ZoneId { get; set; }
    }
}
