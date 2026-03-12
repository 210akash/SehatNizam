using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class GetEmployeeLeaveBalanceQuery : IRequest<List<LeaveBalanceDto>>
    {
    }
}