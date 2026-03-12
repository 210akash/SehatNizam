using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Query
{
    public class GetEmployeeGradeByNameQuery : IRequest<List<GetEmployeeGrade>>
    {
        public GetEmployeeGradeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}