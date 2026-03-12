using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Command
{
    public class SavePriceGroupCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
