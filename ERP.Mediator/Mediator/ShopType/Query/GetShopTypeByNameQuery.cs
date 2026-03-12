using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Query
{
    public class GetShopTypeByNameQuery : IRequest<List<GetShopType>>
    {
        public GetShopTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}