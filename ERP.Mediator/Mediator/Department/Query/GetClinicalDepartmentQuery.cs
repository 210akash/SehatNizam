using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class GetClinicalDepartmentQuery : IRequest<List<GetDepartment>>
    {
    }
}