using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Command
{
    public class SaveEmployeeGradeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
