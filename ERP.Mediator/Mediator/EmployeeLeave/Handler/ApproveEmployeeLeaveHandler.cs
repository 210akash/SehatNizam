using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeave.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class ApproveEmployeeLeaveHandler : IRequestHandler<ApproveEmployeeLeaveCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveEmployeeLeaveHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<ApproveEmployeeLeaveCommand, long>.Handle(ApproveEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            var employeeLeave = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetFirstAsNoTrackingAsync(x => x.IsActive && !x.IsDelete && x.Id == request.Id);
            employeeLeave.StatusId = 160;
            employeeLeave.Comments = request.Comments;
            employeeLeave.ApprovedById = sessionProvider.Session.LoggedInUserId;
            employeeLeave.ApprovedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.EmployeeLeave>().Update(employeeLeave);
            SaveChanges();
            return 200;

        }
    }
}