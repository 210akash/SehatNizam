using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeEducation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Handler
{
    public class DeleteEmployeeEducationHandler : IRequestHandler<DeleteEmployeeEducationQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeEducationHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeEducationQuery request, CancellationToken cancellationToken)
        {
            var employeeEducation = await unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeEducation.IsDelete = true;
            employeeEducation.IsActive = false;
            employeeEducation.DeleteDate = DateTime.Now;
            employeeEducation.ModifiedDate = DateTime.Now;
            employeeEducation.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeEducation>().Update(employeeEducation);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
