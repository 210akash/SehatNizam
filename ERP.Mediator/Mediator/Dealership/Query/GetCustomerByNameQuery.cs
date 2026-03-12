using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class GetCustomerByNameQuery : IRequest<List<GetDealership>>
    {
        public GetCustomerByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}