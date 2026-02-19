using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Query
{
    public class GetLocationByIdQuery : IRequest<GetLocation>
    {
        public GetLocationByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}