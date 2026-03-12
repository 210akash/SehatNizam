using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Command
{
    public class SaveVoucherTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long AccountHeadId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
