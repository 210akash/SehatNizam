using MediatR;
using System;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class RejectDispatchQuery : IRequest<Tuple<long, string>>
    {
        public RejectDispatchQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}