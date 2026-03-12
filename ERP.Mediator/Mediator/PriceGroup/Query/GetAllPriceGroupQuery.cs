using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Query
{
    public class GetAllPriceGroupQuery : IRequest<Tuple<IEnumerable<GetPriceGroup>, long>>
    {
        public string Title { get; set; }

        public PagingData PagingData { get; set; }
    }
}