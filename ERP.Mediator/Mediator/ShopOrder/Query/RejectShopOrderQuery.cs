using MediatR;
using System;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class RejectShopOrderQuery : IRequest<bool>
    {
        public RejectShopOrderQuery(long Id,Guid UserId, string Remarks)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.Remarks = Remarks;
        }

        public long Id { get; set; }
        public Guid UserId { get; set; }
        public string Remarks { get; set; }
    }
}