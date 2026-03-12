using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Query
{
    public class DeleteSubCategoryQuery : IRequest<bool>
    {
        public DeleteSubCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}