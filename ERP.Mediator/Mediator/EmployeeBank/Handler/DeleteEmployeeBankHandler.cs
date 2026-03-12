using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeBank.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Handler
{
    public class DeleteEmployeeBankHandler : IRequestHandler<DeleteEmployeeBankQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeBankHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeBankQuery request, CancellationToken cancellationToken)
        {
            var employeeBank = await unitOfWork.Repository<Entities.Models.EmployeeBank>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeBank.IsDelete = true;
            employeeBank.IsActive = false;
            employeeBank.DeleteDate = DateTime.Now;
            employeeBank.ModifiedDate = DateTime.Now;
            employeeBank.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeBank>().Update(employeeBank);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
