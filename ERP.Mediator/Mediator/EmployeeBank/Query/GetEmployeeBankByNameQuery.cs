using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Query
{
    public class GetEmployeeBankByNameQuery : IRequest<List<GetEmployeeBank>>
    {
        public GetEmployeeBankByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}