using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Referrer.Query
{
    public class GetReferrerByNameQuery : IRequest<List<GetReferrer>>
    {
        public GetReferrerByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}