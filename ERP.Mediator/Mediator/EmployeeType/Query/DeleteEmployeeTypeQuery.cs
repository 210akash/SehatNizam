using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Query
{
    public class DeleteEmployeeTypeQuery : IRequest<bool>
    {
        public DeleteEmployeeTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}