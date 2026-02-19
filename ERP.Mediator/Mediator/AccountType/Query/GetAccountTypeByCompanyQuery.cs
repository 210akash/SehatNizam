using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Query
{
    public class GetAccountTypeByCompanyQuery : IRequest<List<GetAccountType>>
    {
        public GetAccountTypeByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}