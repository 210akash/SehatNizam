using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Query
{
    public class GetEmployeeDocumentTypeByIdQuery : IRequest<GetEmployeeDocumentType>
    {
        public GetEmployeeDocumentTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}