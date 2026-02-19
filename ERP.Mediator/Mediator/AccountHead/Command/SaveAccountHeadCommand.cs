using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Command
{
    public class SaveAccountHeadCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
