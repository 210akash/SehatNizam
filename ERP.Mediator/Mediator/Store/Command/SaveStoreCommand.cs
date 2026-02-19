using MediatR;

namespace ERP.Mediator.Mediator.Store.Command
{
    public class SaveStoreCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool FixedAsset { get; set; }
    }
}
