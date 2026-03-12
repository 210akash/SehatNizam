using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetShopByNameQuery : IRequest<List<GetShop>>
    {
        public GetShopByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}