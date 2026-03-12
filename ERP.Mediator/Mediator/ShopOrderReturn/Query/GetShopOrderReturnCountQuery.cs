using System;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Query
{
    public class GetShopOrderReturnCountQuery : IRequest<Tuple<long, long>>
    {
        public string Code { get; set; }
        public string ShopOrderReturnId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}