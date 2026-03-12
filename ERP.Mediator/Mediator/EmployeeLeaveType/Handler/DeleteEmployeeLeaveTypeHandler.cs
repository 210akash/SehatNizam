using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Handler
{
    public class DeleteEmployeeLeaveTypeHandler : IRequestHandler<DeleteEmployeeLeaveTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeLeaveTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveType = await unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeLeaveType.IsDelete = true;
            employeeLeaveType.IsActive = false;
            employeeLeaveType.DeleteDate = DateTime.Now;
            employeeLeaveType.ModifiedDate = DateTime.Now;
            employeeLeaveType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().Update(employeeLeaveType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
