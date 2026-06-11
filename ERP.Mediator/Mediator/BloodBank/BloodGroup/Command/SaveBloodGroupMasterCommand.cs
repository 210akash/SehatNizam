using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodGroup.Command
{
    public class SaveBloodGroupMasterCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
