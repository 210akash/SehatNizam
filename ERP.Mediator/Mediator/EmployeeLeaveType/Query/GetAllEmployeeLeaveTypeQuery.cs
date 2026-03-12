using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Query
{
    public class GetAllEmployeeLeaveTypeQuery : IRequest<Tuple<IEnumerable<GetEmployeeLeaveType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}