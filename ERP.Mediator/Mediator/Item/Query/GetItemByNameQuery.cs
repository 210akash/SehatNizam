using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetItemByNameQuery : IRequest<List<GetItem>>
    {
        public GetItemByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}