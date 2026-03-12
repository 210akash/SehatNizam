using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Query
{
    public class GetVendorByIdQuery : IRequest<GetVendor>
    {
        public GetVendorByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}