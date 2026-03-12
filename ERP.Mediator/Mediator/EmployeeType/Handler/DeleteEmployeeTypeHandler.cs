using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Handler
{
    public class DeleteEmployeeTypeHandler : IRequestHandler<DeleteEmployeeTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeTypeQuery request, CancellationToken cancellationToken)
        {
            var employeeType = await unitOfWork.Repository<Entities.Models.EmployeeType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeType.IsDelete = true;
            employeeType.IsActive = false;
            employeeType.DeleteDate = DateTime.Now;
            employeeType.ModifiedDate = DateTime.Now;
            employeeType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeType>().Update(employeeType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
