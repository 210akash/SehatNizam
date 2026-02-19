using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class GetItemTypeByNameQuery : IRequest<List<GetItemType>>
    {
        public GetItemTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}