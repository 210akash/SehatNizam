using MediatR;

namespace ERP.Mediator.Mediator.Area.Command
{
    public class SaveAreaCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public long? ZoneId { get; set; }
    }
}
