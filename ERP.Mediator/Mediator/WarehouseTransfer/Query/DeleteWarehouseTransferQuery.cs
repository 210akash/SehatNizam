using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Query
{
    public class DeleteWarehouseTransferQuery : IRequest<bool>
    {
        public DeleteWarehouseTransferQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}