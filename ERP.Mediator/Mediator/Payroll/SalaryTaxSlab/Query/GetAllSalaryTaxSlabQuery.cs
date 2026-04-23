using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Query
{
    public class GetAllSalaryTaxSlabQuery : IRequest<Tuple<IEnumerable<GetSalaryTaxSlab>, long>>
    {
        public PagingData PagingData { get; set; }
    }
}