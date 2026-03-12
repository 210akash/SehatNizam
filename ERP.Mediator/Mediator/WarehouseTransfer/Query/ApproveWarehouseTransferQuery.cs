using System;
using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Query
{
    public class ApproveWarehouseTransferQuery : IRequest<Tuple<long, string>>
    {
        public ApproveWarehouseTransferQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}