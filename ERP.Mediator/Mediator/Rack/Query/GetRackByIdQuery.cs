using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Query
{
    public class GetRackByIdQuery : IRequest<GetRack>
    {
        public GetRackByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}