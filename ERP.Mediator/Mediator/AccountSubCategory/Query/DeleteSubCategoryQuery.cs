using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Query
{
    public class DeleteAccountSubCategoryQuery : IRequest<bool>
    {
        public DeleteAccountSubCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}