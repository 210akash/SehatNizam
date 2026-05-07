using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Query
{
    public class GetServiceByIdQuery : IRequest<GetService>
    {
        public long Id { get; set; }

        public GetServiceByIdQuery(long id)
        {
            Id = id;
        }
    }
}
