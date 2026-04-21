using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Query
{
    public class GetAllSalaryHeadsQuery : IRequest<IEnumerable<GetSalaryHead>>
    {
    }
}
