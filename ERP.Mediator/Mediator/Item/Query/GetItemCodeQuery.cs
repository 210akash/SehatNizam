using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetItemCodeQuery : IRequest<string>
    {
        public GetItemCodeQuery(long ItemTypeId,long Id)
        {
            this.ItemTypeId = ItemTypeId;
            this.Id = Id;
        }
        public long ItemTypeId { get; set; }
        public long Id { get; set; }
    }

}