using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Query
{
    public class GetAllCompanyQuery : IRequest<Tuple<IEnumerable<GetCompany>, long>>
    {
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}