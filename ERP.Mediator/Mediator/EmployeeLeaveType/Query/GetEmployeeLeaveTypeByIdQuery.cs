using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Query
{
    public class GetEmployeeLeaveTypeByIdQuery : IRequest<GetEmployeeLeaveType>
    {
        public GetEmployeeLeaveTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}