using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class GetAllDepartmentQuery : IRequest<Tuple<IEnumerable<GetDepartment>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}