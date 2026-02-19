using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Query
{
    public class GetDepartmentByIdQuery : IRequest<GetDepartment>
    {
        public GetDepartmentByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}