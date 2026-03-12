using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeShift.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Handler
{
    public class SaveEmployeeShiftHandler : IRequestHandler<SaveEmployeeShiftCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeShiftHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeShiftCommand, long>.Handle(SaveEmployeeShiftCommand request, CancellationToken cancellationToken)
        {
            var employeeShift = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeShift == null)
                {
                    var _employeeShift = mapper.Map<Entities.Models.EmployeeShift>(request);
                    _employeeShift.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeShift.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeShift>().Add(_employeeShift);
                    SaveChanges();
                }
                else
                {
                    var _employeeShift = mapper.Map<Entities.Models.EmployeeShift>(request);
                    _employeeShift.CreatedById = employeeShift.CreatedById;
                    _employeeShift.CreatedDate = employeeShift.CreatedDate;
                    _employeeShift.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeShift.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeShift>().Update(_employeeShift);
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