using MediatR;
using System;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class ApproveIssuanceQuery : IRequest<Tuple<long, string>>
    {
        public ApproveIssuanceQuery(long Id,string StatusRemarks)
        {
            this.Id = Id;
            this.StatusRemarks = StatusRemarks;
        }

        public long Id { get; set; }
        public string StatusRemarks { get; set; }
    }
}