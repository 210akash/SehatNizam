using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class GetDepartmentByCompanyQuery : IRequest<List<GetDepartment>>
    {
        public GetDepartmentByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}