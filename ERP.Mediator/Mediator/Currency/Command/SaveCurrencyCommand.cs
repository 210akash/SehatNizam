using MediatR;

namespace ERP.Mediator.Mediator.Currency.Command
{
    public class SaveCurrencyCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
