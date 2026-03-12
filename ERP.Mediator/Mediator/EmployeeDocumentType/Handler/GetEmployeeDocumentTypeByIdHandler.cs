using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeDocumentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Handler
{
    public class GetEmployeeDocumentTypeByIdHandler : IRequestHandler<GetEmployeeDocumentTypeByIdQuery, GetEmployeeDocumentType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeDocumentTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeDocumentType> Handle(GetEmployeeDocumentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeDocumentType = await unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeDocumentType = mapper.Map<GetEmployeeDocumentType>(employeeDocumentType);
            return _employeeDocumentType;
        }
    }
}
