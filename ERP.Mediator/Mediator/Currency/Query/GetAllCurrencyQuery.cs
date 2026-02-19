using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Query
{
    public class GetAllCurrencyQuery : IRequest<Tuple<IEnumerable<GetCurrency>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}