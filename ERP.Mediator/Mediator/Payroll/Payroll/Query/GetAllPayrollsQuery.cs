using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Query
{
    public class GetAllPayrollsQuery : IRequest<IEnumerable<GetPayroll>>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
