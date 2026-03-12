using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetDispatchByIdQuery : IRequest<GetDispatch>
    {
        public GetDispatchByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}