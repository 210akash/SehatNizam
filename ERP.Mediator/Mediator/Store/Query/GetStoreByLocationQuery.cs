using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class GetStoreByLocationQuery : IRequest<List<GetStore>>
    {
        public GetStoreByLocationQuery(long LocationId)
        {
            this.LocationId = LocationId;
        }

        public long LocationId { get; set; }
    }
}