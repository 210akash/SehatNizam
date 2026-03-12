using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Query
{
    public class GetSectionByNameQuery : IRequest<List<GetSection>>
    {
        public GetSectionByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}