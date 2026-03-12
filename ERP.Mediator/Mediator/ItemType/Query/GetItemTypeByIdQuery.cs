using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class GetItemTypeByIdQuery : IRequest<GetItemType>
    {
        public GetItemTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}