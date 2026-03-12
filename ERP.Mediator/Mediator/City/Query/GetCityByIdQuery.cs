using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.City.Query
{
    public class GetCityByIdQuery : IRequest<GetCity>
    {
        public GetCityByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}