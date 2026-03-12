using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Query
{
    public class DeleteEmployeeOvertimeRateQuery : IRequest<bool>
    {
        public DeleteEmployeeOvertimeRateQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}