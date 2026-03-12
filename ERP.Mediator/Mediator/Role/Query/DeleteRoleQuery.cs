using MediatR;

namespace ERP.Mediator.Mediator.Role.Query
{
    public class DeleteRoleQuery : IRequest<long>
    {
        public DeleteRoleQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }
    }
}