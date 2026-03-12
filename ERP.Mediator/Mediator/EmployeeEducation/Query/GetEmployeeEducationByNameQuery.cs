using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Query
{
    public class GetEmployeeEducationByNameQuery : IRequest<List<GetEmployeeEducation>>
    {
        public GetEmployeeEducationByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}