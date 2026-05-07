using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Query
{
    public class GetRadiologyOrderByIdQuery : IRequest<GetRadiologyOrder>
    {
        public long Id { get; set; }

        public GetRadiologyOrderByIdQuery(long id)
        {
            Id = id;
        }
    }
}
