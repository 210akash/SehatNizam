using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Query
{
    public class GetEmployeeTypeByIdQuery : IRequest<GetEmployeeType>
    {
        public GetEmployeeTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}