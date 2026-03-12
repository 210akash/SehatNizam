using System;
using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Query
{
    public class RevokeWarehouseTransferQuery : IRequest<Tuple<long, string>>
    {
        public RevokeWarehouseTransferQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}