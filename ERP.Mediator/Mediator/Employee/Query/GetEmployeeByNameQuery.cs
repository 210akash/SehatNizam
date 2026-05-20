using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Employee.Query
{
    public class GetEmployeeByNameQuery : IRequest<List<GetEmployee>>
    {
        public GetEmployeeByNameQuery(string Name, long? DepartmentId)
        {
            this.Name = Name;
            this.DepartmentId = DepartmentId;
        }

        public string Name { get; set; }
        public long? DepartmentId { get; set; }
    }
}