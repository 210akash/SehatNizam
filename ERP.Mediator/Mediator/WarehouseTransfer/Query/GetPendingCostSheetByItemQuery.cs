using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Query
{
    public class GetPendingCostSheetByItemQuery : IRequest<List<GetCostSheet>>
    {
        public GetPendingCostSheetByItemQuery(long ItemId, long ProjectId, long CostSheetId)
        {
            this.ItemId = ItemId;
            this.ProjectId = ProjectId;
            this.CostSheetId = CostSheetId;
        }

        public long ItemId { get; set; }
        public long ProjectId { get; set; }
        public long CostSheetId { get; set; }
    }
}