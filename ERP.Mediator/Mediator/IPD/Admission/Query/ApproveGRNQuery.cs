using MediatR;
using System.Threading.Tasks;
using System;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class ApproveGRNQuery : IRequest<Tuple<long, string>>
    {
        public ApproveGRNQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}