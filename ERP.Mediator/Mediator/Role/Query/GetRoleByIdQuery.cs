using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Query
{
    public class GetRoleByIdQuery : IRequest<GetRoles>
    {
        public GetRoleByIdQuery(string Id)
        {
            this.Id = Id;
        }

        public string Id { get; set; }
    }
}