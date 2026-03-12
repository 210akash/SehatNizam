using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class DeleteEmployeeLeaveHandler : IRequestHandler<DeleteEmployeeLeaveQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeLeaveHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeLeaveQuery request, CancellationToken cancellationToken)
        {
            var employeeLeave = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeLeave.IsDelete = true;
            employeeLeave.IsActive = false;
            employeeLeave.DeleteDate = DateTime.Now;
            employeeLeave.ModifiedDate = DateTime.Now;
            employeeLeave.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeLeave>().Update(employeeLeave);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
