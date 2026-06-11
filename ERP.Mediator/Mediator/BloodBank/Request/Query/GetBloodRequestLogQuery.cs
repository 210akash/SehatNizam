using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Query
{
    public class GetBloodRequestLogQuery : IRequest<GetBloodRequestLog>
    {
        public long BloodRequestId { get; set; }
        public GetBloodRequestLogQuery(long bloodRequestId) { BloodRequestId = bloodRequestId; }
    }
}
