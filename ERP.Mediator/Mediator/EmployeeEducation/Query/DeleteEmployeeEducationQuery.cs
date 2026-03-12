using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Query
{
    public class DeleteEmployeeEducationQuery : IRequest<bool>
    {
        public DeleteEmployeeEducationQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}