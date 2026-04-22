using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Handler
{
    public class DeleteEmployeeSalaryHandler : IRequestHandler<DeleteEmployeeSalaryCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeSalaryHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeSalaryCommand request, CancellationToken cancellationToken)
        {
            var employeeSalary = await unitOfWork.Repository<Entities.Models.EmployeeSalary>().GetFirstAsync(x => x.Id == request.Id);
            if (employeeSalary == null)
            {
                return false;
            }

            employeeSalary.IsDelete = true;
            employeeSalary.IsActive = false;
            employeeSalary.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
            employeeSalary.DeleteDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.EmployeeSalary>().Update(employeeSalary);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
