using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeDocumentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Handler
{
    public class GetEmployeeDocumentByEmployeeIdHandler : IRequestHandler<GetEmployeeDocumentByEmployeeIdQuery, List<GetEmployeeDocument>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeDocumentByEmployeeIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeDocument>> Handle(GetEmployeeDocumentByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            var employeeDocumentType = await unitOfWork.Repository<Entities.Models.EmployeeDocument>().GetAsync(y => y.EmployeeId == request.EmployeeId);
            var _employeeDocumentType = mapper.Map<List<GetEmployeeDocument>>(employeeDocumentType);
            return _employeeDocumentType;
        }
    }
}
