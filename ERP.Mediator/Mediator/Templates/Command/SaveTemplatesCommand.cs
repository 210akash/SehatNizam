using MediatR;

namespace ERP.Mediator.Mediator.Templates.Command
{
    public class SaveTemplatesCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
