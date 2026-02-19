using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetItemByIdQuery : IRequest<GetItem>
    {
        public GetItemByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}