using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeDesignation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Handler
{
    public class GetEmployeeDesignationByIdHandler : IRequestHandler<GetEmployeeDesignationByIdQuery, GetEmployeeDesignation>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeDesignationByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeDesignation> Handle(GetEmployeeDesignationByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeDesignation = await unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeDesignation = mapper.Map<GetEmployeeDesignation>(employeeDesignation);
            return _employeeDesignation;
        }
    }
}
