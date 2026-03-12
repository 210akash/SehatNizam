using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Query
{
    public class ProcessWarehouseTransferQuery : IRequest<bool>
    {
        public ProcessWarehouseTransferQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}