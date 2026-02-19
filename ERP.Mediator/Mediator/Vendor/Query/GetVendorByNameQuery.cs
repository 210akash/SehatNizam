using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Query
{
    public class GetVendorByNameQuery : IRequest<List<GetVendor>>
    {
        public GetVendorByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}