using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Query
{
    public class GetAllEmployeeOvertimeRateQuery : IRequest<Tuple<IEnumerable<GetEmployeeOvertimeRate>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}