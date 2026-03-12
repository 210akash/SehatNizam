using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeEducation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Handler
{
    public class GetEmployeeEducationByIdHandler : IRequestHandler<GetEmployeeEducationByIdQuery, GetEmployeeEducation>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeEducationByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeEducation> Handle(GetEmployeeEducationByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeEducation = await unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeEducation = mapper.Map<GetEmployeeEducation>(employeeEducation);
            return _employeeEducation;
        }
    }
}
