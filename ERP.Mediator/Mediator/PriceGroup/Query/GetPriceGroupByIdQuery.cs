using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.RegPriceGroupion.Query
{
    public class GetPriceGroupByIdQuery : IRequest<GetRegion>
    {
        public GetPriceGroupByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}