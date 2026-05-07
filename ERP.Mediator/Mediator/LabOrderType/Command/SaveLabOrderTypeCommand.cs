using MediatR;

namespace ERP.Mediator.Mediator.LabOrderType.Command
{
    public class SaveLabOrderTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomFieldsSchema { get; set; }
        public long ServiceId { get; set; }
    }
}
