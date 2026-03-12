using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Query
{
    public class DeleteEmployeeShiftQuery : IRequest<bool>
    {
        public DeleteEmployeeShiftQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}