using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Query
{
    public class GetCompanyByNameQuery : IRequest<List<GetCompany>>
    {
        public GetCompanyByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}