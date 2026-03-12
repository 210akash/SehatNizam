using MediatR;

namespace ERP.Mediator.Mediator.Auth.Command
{
    public class AddRoleCommand : IRequest<long>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}