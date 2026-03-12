using MediatR;

namespace ERP.Mediator.Mediator.Region.Command
{
    public class SaveRegionCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
    }
}
