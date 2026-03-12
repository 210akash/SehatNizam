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
    public class GetEmployeeDocumentTypeByNameHandler : IRequestHandler<GetEmployeeDocumentTypeByNameQuery, List<GetEmployeeDocumentType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeDocumentTypeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeDocumentType>> Handle(GetEmployeeDocumentTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeDocumentType = await unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetAsync(y => y.Name == request.name);
            var _employeeDocumentType = mapper.Map<List<GetEmployeeDocumentType>>(employeeDocumentType);
            return _employeeDocumentType;
        }
    }
}
