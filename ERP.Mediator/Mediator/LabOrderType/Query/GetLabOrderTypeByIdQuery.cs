using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrderType.Query
{
    public class GetLabOrderTypeByIdQuery : IRequest<GetLabOrderType>
    {
        public GetLabOrderTypeByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
