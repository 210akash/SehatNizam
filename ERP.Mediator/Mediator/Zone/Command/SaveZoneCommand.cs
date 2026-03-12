using MediatR;

namespace ERP.Mediator.Mediator.Zone.Command
{
    public class SaveZoneCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public long? RegionId { get; set; }
    }
}
