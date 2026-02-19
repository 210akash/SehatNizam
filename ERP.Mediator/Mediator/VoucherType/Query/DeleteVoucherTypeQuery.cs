using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Query
{
    public class DeleteVoucherTypeQuery : IRequest<bool>
    {
        public DeleteVoucherTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}