using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Query
{
    public class GetOrderByIdQuery : IRequest<GetOrder>
    {
        public GetOrderByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}