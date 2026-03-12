using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetItemByNameQuery : IRequest<List<GetItem>>
    {
        public GetItemByNameQuery(string name, long StoreId)
        {
            this.name = name;
            this.StoreId = StoreId;
        }

        public string name { get; set; }
        public long StoreId { get; set; }
    }
}