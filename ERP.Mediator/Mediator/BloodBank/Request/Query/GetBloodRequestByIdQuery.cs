using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Query
{
    public class GetBloodRequestByIdQuery : IRequest<GetBloodRequest>
    {
        public long Id { get; set; }
        public GetBloodRequestByIdQuery(long id) { Id = id; }
    }
}
