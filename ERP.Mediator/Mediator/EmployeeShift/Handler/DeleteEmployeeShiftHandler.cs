using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeShift.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Handler
{
    public class DeleteEmployeeShiftHandler : IRequestHandler<DeleteEmployeeShiftQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeShiftHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeShiftQuery request, CancellationToken cancellationToken)
        {
            var employeeShift = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeShift.IsDelete = true;
            employeeShift.IsActive = false;
            employeeShift.DeleteDate = DateTime.Now;
            employeeShift.ModifiedDate = DateTime.Now;
            employeeShift.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeShift>().Update(employeeShift);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
