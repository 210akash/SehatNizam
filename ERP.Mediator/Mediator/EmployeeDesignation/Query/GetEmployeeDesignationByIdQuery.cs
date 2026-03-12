using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Query
{
    public class GetEmployeeDesignationByIdQuery : IRequest<GetEmployeeDesignation>
    {
        public GetEmployeeDesignationByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}