using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class GetShopOrderByDealershipQuery : IRequest<Tuple<IEnumerable<GetShopOrder>, long>>
    {
        public long? ShopOrderId { get; set; }
        public long? StatusId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public DateTime? AppDateTime { get; set; }
        public long DealershipId { get; set; }
        public long? ShopId { get; set; }
        public PagingData PagingData { get; set; }
    }
}