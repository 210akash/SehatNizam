using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Query
{
    public class GetAllVendorQuery : IRequest<Tuple<IEnumerable<GetVendor>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}