using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Command
{
    public class SaveRadiologyTypeCommand : IRequest<int>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ServiceId { get; set; }
        public long CompanyId { get; set; }
    }
}
