using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Command
{
    public class SaveIndentTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
