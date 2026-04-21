using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Query
{
    public class GetSalaryHeadByIdQuery : IRequest<GetSalaryHead>
    {
        public long Id { get; set; }

        public GetSalaryHeadByIdQuery(long id)
        {
            Id = id;
        }
    }
}
