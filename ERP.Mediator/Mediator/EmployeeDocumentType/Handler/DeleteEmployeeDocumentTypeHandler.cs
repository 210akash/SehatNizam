using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDocumentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Handler
{
    public class DeleteEmployeeDocumentTypeHandler : IRequestHandler<DeleteEmployeeDocumentTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeDocumentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            var employeeDocumentType = await unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeDocumentType.IsDelete = true;
            employeeDocumentType.IsActive = false;
            employeeDocumentType.DeleteDate = DateTime.Now;
            employeeDocumentType.ModifiedDate = DateTime.Now;
            employeeDocumentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().Update(employeeDocumentType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
