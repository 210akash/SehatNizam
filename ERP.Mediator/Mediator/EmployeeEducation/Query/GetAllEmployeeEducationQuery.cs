using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Query
{
    public class GetAllEmployeeEducationQuery : IRequest<Tuple<IEnumerable<GetEmployeeEducation>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}