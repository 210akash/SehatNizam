using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Query
{
    public class GetAllEmployeeBankQuery : IRequest<Tuple<IEnumerable<GetEmployeeBank>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}