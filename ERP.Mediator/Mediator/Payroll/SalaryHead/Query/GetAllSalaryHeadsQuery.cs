using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Query
{
    public class GetAllSalaryHeadsQuery : IRequest<Tuple<IEnumerable<GetSalaryHead>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}
