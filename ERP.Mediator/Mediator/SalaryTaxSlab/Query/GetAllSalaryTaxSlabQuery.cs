using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.SalaryTaxSlab.Query
{
    public class GetAllSalaryTaxSlabQuery : IRequest<Tuple<IEnumerable<GetSalaryTaxSlab>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}