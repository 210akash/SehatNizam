using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Query
{
    public class GetAllEmployeeShiftQuery : IRequest<Tuple<IEnumerable<GetEmployeeShift>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}