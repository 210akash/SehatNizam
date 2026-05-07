using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Query
{
    public class GetLabOrderByIdQuery : IRequest<GetLabOrder>
    {
        public GetLabOrderByIdQuery(long id) { Id = id; }
        public long Id { get; set; }
    }
}
