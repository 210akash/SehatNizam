using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class DeleteEmployeeLeaveGroupHandler : IRequestHandler<DeleteEmployeeLeaveGroupQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeLeaveGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeLeaveGroupQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveGroup = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeLeaveGroup.IsDelete = true;
            employeeLeaveGroup.IsActive = false;
            employeeLeaveGroup.DeleteDate = DateTime.Now;
            employeeLeaveGroup.ModifiedDate = DateTime.Now;
            employeeLeaveGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().Update(employeeLeaveGroup);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
