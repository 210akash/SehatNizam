using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDesignation.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Handler
{
    public class SaveEmployeeDesignationHandler : IRequestHandler<SaveEmployeeDesignationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeDesignationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeDesignationCommand, long>.Handle(SaveEmployeeDesignationCommand request, CancellationToken cancellationToken)
        {
            var employeeDesignation = await unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeDesignation == null)
                {
                    var _employeeDesignation = mapper.Map<Entities.Models.EmployeeDesignation>(request);
                    _employeeDesignation.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDesignation.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeDesignation>().Add(_employeeDesignation);
                    SaveChanges();
                }
                else
                {
                    var _employeeDesignation = mapper.Map<Entities.Models.EmployeeDesignation>(request);
                    _employeeDesignation.CreatedById = employeeDesignation.CreatedById;
                    _employeeDesignation.CreatedDate = employeeDesignation.CreatedDate;
                    _employeeDesignation.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDesignation.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeDesignation>().Update(_employeeDesignation);
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