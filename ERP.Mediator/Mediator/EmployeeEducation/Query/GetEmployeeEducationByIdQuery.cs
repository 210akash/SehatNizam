using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Query
{
    public class GetEmployeeEducationByIdQuery : IRequest<GetEmployeeEducation>
    {
        public GetEmployeeEducationByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}