using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Query
{
    public class GetEmployeeGradeByIdQuery : IRequest<GetEmployeeGrade>
    {
        public GetEmployeeGradeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}