using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Query
{
    public class GetCategoryByNameQuery : IRequest<List<GetCategory>>
    {
        public GetCategoryByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}