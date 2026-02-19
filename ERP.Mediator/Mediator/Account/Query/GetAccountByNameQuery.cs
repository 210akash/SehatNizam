using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Query
{
    public class GetAccountByNameQuery : IRequest<List<GetAccount>>
    {
        public GetAccountByNameQuery(string name, List<string> accountFlow)
        {
            this.name = name;
            this.accountFlow = accountFlow;
        }

        public string name { get; set; }
        public List<string> accountFlow { get; set; }
    }
}