using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeOvertimeRate.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Handler
{
    public class DeleteEmployeeOvertimeRateHandler : IRequestHandler<DeleteEmployeeOvertimeRateQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteEmployeeOvertimeRateHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteEmployeeOvertimeRateQuery request, CancellationToken cancellationToken)
        {
            var EmployeeOvertimeRate = await unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            EmployeeOvertimeRate.IsDelete = true;
            EmployeeOvertimeRate.IsActive = false;
            EmployeeOvertimeRate.DeleteDate = DateTime.Now;
            EmployeeOvertimeRate.ModifiedDate = DateTime.Now;
            EmployeeOvertimeRate.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().Update(EmployeeOvertimeRate);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
