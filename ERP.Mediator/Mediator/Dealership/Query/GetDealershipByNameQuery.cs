using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetDealershipByNameQuery : IRequest<List<GetDealership>>
    {
        public GetDealershipByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}