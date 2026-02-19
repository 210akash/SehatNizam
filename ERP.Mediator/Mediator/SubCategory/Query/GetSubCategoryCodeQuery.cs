using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class GetSubCategoryCodeQuery : IRequest<string>
    {
        public GetSubCategoryCodeQuery(long CategoryId,long Id)
        {
            this.CategoryId = CategoryId;
            this.Id = Id;
        }
        public long CategoryId { get; set; }
        public long Id { get; set; }
    }

}