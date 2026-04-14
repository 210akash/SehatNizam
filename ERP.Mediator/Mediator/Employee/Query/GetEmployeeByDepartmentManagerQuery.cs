using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Employee.Query
{
    public class GetEmployeeByDepartmentManagerQuery : IRequest<List<GetEmployee>>
    {
    }
}