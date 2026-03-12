using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDesignation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Handler
{
    public class DeleteEmployeeDesignationHandler : IRequestHandler<DeleteEmployeeDesignationQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeDesignationHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeDesignationQuery request, CancellationToken cancellationToken)
        {
            var employeeDesignation = await unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeDesignation.IsDelete = true;
            employeeDesignation.IsActive = false;
            employeeDesignation.DeleteDate = DateTime.Now;
            employeeDesignation.ModifiedDate = DateTime.Now;
            employeeDesignation.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeDesignation>().Update(employeeDesignation);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
