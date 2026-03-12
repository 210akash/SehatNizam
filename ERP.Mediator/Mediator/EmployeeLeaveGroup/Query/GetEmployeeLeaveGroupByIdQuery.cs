using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Query
{
    public class GetEmployeeLeaveGroupByIdQuery : IRequest<GetEmployeeLeaveGroup>
    {
        public GetEmployeeLeaveGroupByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}