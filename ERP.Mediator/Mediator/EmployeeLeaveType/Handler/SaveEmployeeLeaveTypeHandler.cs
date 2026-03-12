using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Handler
{
    public class SaveEmployeeLeaveTypeHandler : IRequestHandler<SaveEmployeeLeaveTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeLeaveTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeLeaveTypeCommand, long>.Handle(SaveEmployeeLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            var employeeLeaveType = await unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeLeaveType == null)
                {
                    var _employeeLeaveType = mapper.Map<Entities.Models.EmployeeLeaveType>(request);
                    _employeeLeaveType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeLeaveType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().Add(_employeeLeaveType);
                    SaveChanges();
                }
                else
                {
                    var _employeeLeaveType = mapper.Map<Entities.Models.EmployeeLeaveType>(request);
                    _employeeLeaveType.CreatedById = employeeLeaveType.CreatedById;
                    _employeeLeaveType.CreatedDate = employeeLeaveType.CreatedDate;
                    _employeeLeaveType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeLeaveType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().Update(_employeeLeaveType);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}