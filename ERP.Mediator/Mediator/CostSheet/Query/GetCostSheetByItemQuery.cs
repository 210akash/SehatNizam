using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Company.Query
{
    public class GetCostSheetByItemQuery : IRequest<List<GetDropDown>>
    {
        public GetCostSheetByItemQuery(long ItemId)
        {
            this.ItemId = ItemId;
        }

        public long ItemId { get; set; }
    }
}