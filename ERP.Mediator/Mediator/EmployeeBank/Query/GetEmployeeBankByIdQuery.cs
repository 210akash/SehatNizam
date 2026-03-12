using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Query
{
    public class GetEmployeeBankByIdQuery : IRequest<GetEmployeeBank>
    {
        public GetEmployeeBankByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}