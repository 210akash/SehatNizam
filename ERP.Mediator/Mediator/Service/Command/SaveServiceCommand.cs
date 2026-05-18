using MediatR;

namespace ERP.Mediator.Mediator.Service.Command
{
    public class SaveServiceCommand : IRequest<int>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public long? DepartmentId { get; set; }
        public long ServiceTypeId { get; set; }
    }
}
