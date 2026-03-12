using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class GetZoneByIdQuery : IRequest<GetZone>
    {
        public GetZoneByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}