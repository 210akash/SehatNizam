using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.City.Query
{
    public class GetCityByNameQuery : IRequest<List<GetCity>>
    {
        public GetCityByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}