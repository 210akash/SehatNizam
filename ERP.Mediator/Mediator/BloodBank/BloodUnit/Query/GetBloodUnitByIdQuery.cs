using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Query
{
    public class GetBloodUnitByIdQuery : IRequest<GetBloodUnit>
    {
        public long Id { get; set; }
        public GetBloodUnitByIdQuery(long id) { Id = id; }
    }
}
