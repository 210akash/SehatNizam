using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetPendingCostSheetQuery : IRequest<List<GetDropDown>>
    {
        public GetPendingCostSheetQuery(long ItemId, long? CostSheetId)
        {
            this.ItemId = ItemId;
            this.CostSheetId = CostSheetId;
        }

        public long ItemId { get; set; }
        public long? CostSheetId { get; set; }
    }
}