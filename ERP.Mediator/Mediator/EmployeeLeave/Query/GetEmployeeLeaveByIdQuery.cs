using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class GetEmployeeLeaveByIdQuery : IRequest<GetEmployeeLeave>
    {
        public GetEmployeeLeaveByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}