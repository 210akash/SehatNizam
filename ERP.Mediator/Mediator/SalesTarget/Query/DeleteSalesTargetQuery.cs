using MediatR;
using System;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class DeleteSalesTargetQuery : IRequest<long>
    {
        public DeleteSalesTargetQuery(long ZoneId, DateTime TargetMonth)
        {
            this.TargetMonth = TargetMonth;
            this.ZoneId = ZoneId;
        }

        public long ZoneId { get; set; }
        public DateTime TargetMonth { get; set; }
    }
}