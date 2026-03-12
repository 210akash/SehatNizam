using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Query
{
    public class GetAllEmployeeGradeQuery : IRequest<Tuple<IEnumerable<GetEmployeeGrade>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}