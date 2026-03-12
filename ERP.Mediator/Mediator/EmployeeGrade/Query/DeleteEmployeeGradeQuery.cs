using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Query
{
    public class DeleteEmployeeGradeQuery : IRequest<bool>
    {
        public DeleteEmployeeGradeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}