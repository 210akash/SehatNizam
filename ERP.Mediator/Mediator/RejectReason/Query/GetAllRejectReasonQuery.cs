using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.RejectReason.Query
{
    public class GetAllRejectReasonQuery : IRequest<Tuple<IEnumerable<GetRejectReason>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}