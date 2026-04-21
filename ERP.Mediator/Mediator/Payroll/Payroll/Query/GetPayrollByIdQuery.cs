using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Query
{
    public class GetPayrollByIdQuery : IRequest<GetPayroll>
    {
        public long Id { get; set; }

        public GetPayrollByIdQuery(long id)
        {
            Id = id;
        }
    }
}
