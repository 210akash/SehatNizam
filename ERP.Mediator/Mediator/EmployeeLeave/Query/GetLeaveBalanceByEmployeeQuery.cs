using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class GetLeaveBalanceByEmployeeQuery : IRequest<List<LeaveBalanceDto>>
    {
        public GetLeaveBalanceByEmployeeQuery(Guid EmployeeId)
        {
            this.EmployeeId = EmployeeId;
        }

        public Guid EmployeeId { get; set; }
    }
}