using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetDealershipByIdQuery : IRequest<GetDealership>
    {
        public GetDealershipByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}