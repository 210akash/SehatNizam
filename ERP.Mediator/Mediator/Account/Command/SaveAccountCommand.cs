using MediatR;

namespace ERP.Mediator.Mediator.Account.Command
{
    public class SaveAccountCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AccountTypeId { get; set; }
        public long AccountFlowId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }
    }
}
