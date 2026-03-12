using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Query
{
    public class GetCurrencyByIdQuery : IRequest<GetCurrency>
    {
        public GetCurrencyByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}