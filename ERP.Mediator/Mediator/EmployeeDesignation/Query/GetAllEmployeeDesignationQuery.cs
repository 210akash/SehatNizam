using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Query
{
    public class GetAllEmployeeDesignationQuery : IRequest<Tuple<IEnumerable<GetEmployeeDesignation>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}