using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Query
{
    public class GetEmployeeShiftByNameQuery : IRequest<List<GetEmployeeShift>>
    {
        public GetEmployeeShiftByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}