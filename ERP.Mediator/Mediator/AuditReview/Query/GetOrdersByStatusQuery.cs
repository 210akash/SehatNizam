using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AuditReview.Query
{
    public class GetOrdersByStatusQuery : IRequest<Tuple<IEnumerable<GetOrder>, long>>
    {
        public long? StatusId { get; set; }

        public PagingData PagingData { get; set; }
    }
}