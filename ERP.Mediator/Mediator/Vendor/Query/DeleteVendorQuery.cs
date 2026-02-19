using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Query
{
    public class DeleteVendorQuery : IRequest<bool>
    {
        public DeleteVendorQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}