using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetKCItemsQuery : IRequest<List<GetItem>>
    {
        public GetKCItemsQuery(long TerritoryId)
        {
            this.TerritoryId = TerritoryId;
        }

        public long TerritoryId { get; set; }
    }
}