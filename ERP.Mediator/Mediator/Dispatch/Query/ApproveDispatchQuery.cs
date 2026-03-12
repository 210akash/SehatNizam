using MediatR;
using System;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class ApproveDispatchQuery : IRequest<Tuple<long, string>>
    {
        public ApproveDispatchQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}