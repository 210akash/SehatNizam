using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Query
{
    public class GetAllEmployeeLeaveGroupQuery : IRequest<Tuple<IEnumerable<GetEmployeeLeaveGroup>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}