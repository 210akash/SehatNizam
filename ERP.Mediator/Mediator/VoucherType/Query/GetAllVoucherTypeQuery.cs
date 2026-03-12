using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Query
{
    public class GetAllVoucherTypeQuery : IRequest<Tuple<IEnumerable<GetVoucherType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}