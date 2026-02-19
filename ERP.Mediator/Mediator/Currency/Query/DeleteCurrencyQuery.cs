using MediatR;

namespace ERP.Mediator.Mediator.Currency.Query
{
    public class DeleteCurrencyQuery : IRequest<bool>
    {
        public DeleteCurrencyQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}