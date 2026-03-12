using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class DeleteCategoryQuery : IRequest<bool>
    {
        public DeleteCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}