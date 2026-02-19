using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Query
{
    public class GetCurrencyByNameQuery : IRequest<List<GetCurrency>>
    {
        public GetCurrencyByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}