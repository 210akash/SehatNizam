using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeGrade.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Handler
{
    public class DeleteEmployeeGradeHandler : IRequestHandler<DeleteEmployeeGradeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeGradeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeGradeQuery request, CancellationToken cancellationToken)
        {
            var employeeGrade = await unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            employeeGrade.IsDelete = true;
            employeeGrade.IsActive = false;
            employeeGrade.DeleteDate = DateTime.Now;
            employeeGrade.ModifiedDate = DateTime.Now;
            employeeGrade.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeGrade>().Update(employeeGrade);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
