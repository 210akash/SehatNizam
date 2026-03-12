using MediatR;

namespace ERP.Mediator.Mediator.Role.Command
{
    public class SaveRoleCommand : IRequest<long>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccessCheck { get; set; }
    }
}
