using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.ComponentType.Command
{
    public class SaveBloodComponentTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ShelfLifeDays { get; set; }
    }
}
