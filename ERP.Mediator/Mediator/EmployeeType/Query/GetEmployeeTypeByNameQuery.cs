using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Query
{
    public class GetEmployeeTypeByNameQuery : IRequest<List<GetEmployeeType>>
    {
        public GetEmployeeTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}