using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Query
{
    public class GetRadiologyTypeByIdQuery : IRequest<GetRadiologyType>
    {
        public long Id { get; set; }

        public GetRadiologyTypeByIdQuery(long id)
        {
            Id = id;
        }
    }
}
