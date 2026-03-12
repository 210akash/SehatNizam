using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.City.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDevice.Handler
{
    public class GetDevicesByEmployeeHandler : IRequestHandler<GetDevicesByEmployeeQuery, List<GetEmployeeDevice>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDevicesByEmployeeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeDevice>> Handle(GetDevicesByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var EmployeeDevices = await unitOfWork.Repository<Entities.Models.EmployeeDevice>().FindAllAsync(y => y.EmployeeId == request.EmployeeId);
            var _EmployeeDevices = mapper.Map<List<GetEmployeeDevice>>(EmployeeDevices);
            return _EmployeeDevices;
        }
    }
}
