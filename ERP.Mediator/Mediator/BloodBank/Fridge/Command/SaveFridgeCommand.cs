using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Fridge.Command
{
    public class SaveFridgeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
