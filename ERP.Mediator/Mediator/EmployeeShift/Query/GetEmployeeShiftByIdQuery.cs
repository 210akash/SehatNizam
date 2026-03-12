using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Query
{
    public class GetEmployeeShiftByIdQuery : IRequest<GetEmployeeShift>
    {
        public GetEmployeeShiftByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}