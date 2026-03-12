using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Query
{
    public class GetAllRetailOrderQuery : IRequest<Tuple<IEnumerable<GetRetailOrder>, long>>
    {
        public long? RetailOrderId { get; set; }
        public long? StatusId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }

        public PagingData PagingData { get; set; }
    }
}