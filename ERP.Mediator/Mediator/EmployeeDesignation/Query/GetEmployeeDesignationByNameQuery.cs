using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Query
{
    public class GetEmployeeDesignationByNameQuery : IRequest<List<GetEmployeeDesignation>>
    {
        public GetEmployeeDesignationByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}