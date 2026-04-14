using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Employee.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Employee.Handler
{
    public class GetEmployeeByDepartmentManagerHandler : IRequestHandler<GetEmployeeByDepartmentManagerQuery, List<GetEmployee>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetEmployeeByDepartmentManagerHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetEmployee>> Handle(GetEmployeeByDepartmentManagerQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<AspNetUsers>()
            .GetQueryable()
             .Where(e =>
             e.IsActive &&
             e.IsEmployee &&
             e.DepartmentId == this.sessionProvider.Session.DepartmentId)
             .Select(e => new GetEmployee
             {
                 Id = e.Id,
                 HrCode = e.HrCode,
                 PhoneNumber = e.PhoneNumber,
                 FirstName = e.FirstName,
                 LastName = e.LastName,
                 Designation = e.EmployeeDesignation.Name,
                 EmployeeShiftId = e.EmployeeShiftId.Value,
             }).ToListAsync();
            var _Account = mapper.Map<List<GetEmployee>>(result);
            return _Account;
        }
    }
}
