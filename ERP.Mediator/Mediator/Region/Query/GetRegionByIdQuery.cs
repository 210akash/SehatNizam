using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Query
{
    public class GetRegionByIdQuery : IRequest<GetRegion>
    {
        public GetRegionByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}