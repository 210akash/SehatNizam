using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetSockByWarehouseQuery : IRequest<GetStock>
    {
        public GetSockByWarehouseQuery(long ItemId, long ProjectId)
        {
            this.ItemId = ItemId;
            this.ProjectId = ProjectId;
        }

        public long ItemId { get; set; }
        public long ProjectId { get; set; }
    }
}