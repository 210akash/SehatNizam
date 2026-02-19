using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class GetStoreByNameQuery : IRequest<List<GetStore>>
    {
        public GetStoreByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}