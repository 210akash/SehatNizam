using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class GetItemTypeCodeQuery : IRequest<string>
    {
        public GetItemTypeCodeQuery(long SubCategoryId,long Id)
        {
            this.SubCategoryId = SubCategoryId;
            this.Id = Id;
        }
        public long SubCategoryId { get; set; }
        public long Id { get; set; }
    }

}