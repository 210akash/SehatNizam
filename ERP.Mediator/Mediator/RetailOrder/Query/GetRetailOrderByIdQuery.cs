using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Query
{
    public class GetRetailOrderByIdQuery : IRequest<GetRetailOrder>
    {
        public GetRetailOrderByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}