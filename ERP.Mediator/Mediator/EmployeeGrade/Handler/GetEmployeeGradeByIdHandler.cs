using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeGrade.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Handler
{
    public class GetEmployeeGradeByIdHandler : IRequestHandler<GetEmployeeGradeByIdQuery, GetEmployeeGrade>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeGradeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeGrade> Handle(GetEmployeeGradeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeGrade = await unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeGrade = mapper.Map<GetEmployeeGrade>(employeeGrade);
            return _employeeGrade;
        }
    }
}
