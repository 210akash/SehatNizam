using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Command
{
    public class SaveAccountGroupCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long? DealershipId { get; set; }
        public long? VendorId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
