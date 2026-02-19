using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Query
{
    public class GetCurrencyByCompanyQuery : IRequest<List<GetCurrency>>
    {
        public GetCurrencyByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}