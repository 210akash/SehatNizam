using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Query
{
    public class DeleteAccountCategoryQuery : IRequest<bool>
    {
        public DeleteAccountCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}