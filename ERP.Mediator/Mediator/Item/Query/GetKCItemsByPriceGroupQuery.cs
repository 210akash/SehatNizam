using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetKCItemsByPriceGroupQuery : IRequest<List<GetItem>>
    {
        public GetKCItemsByPriceGroupQuery(long TerritoryId, long PriceGroupId)
        {
            this.TerritoryId = TerritoryId;
            this.PriceGroupId = PriceGroupId;
        }

        public long TerritoryId { get; set; }
        public long PriceGroupId { get; set; }
    }
}
