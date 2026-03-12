using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetAccountGroupByNameQuery : IRequest<List<GetAccountGroup>>
    {
        public GetAccountGroupByNameQuery(string name, List<string> accountFlow)
        {
            this.name = name;
            this.accountFlow = accountFlow;
        }

        public string name { get; set; }
        public List<string> accountFlow { get; set; }
    }
}