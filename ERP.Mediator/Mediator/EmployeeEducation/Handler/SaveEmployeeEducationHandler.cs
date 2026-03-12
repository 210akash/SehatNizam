using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeEducation.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Handler
{
    public class SaveEmployeeEducationHandler : IRequestHandler<SaveEmployeeEducationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeEducationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeEducationCommand, long>.Handle(SaveEmployeeEducationCommand request, CancellationToken cancellationToken)
        {
            var employeeEducation = await unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeEducation == null)
                {
                    var _employeeEducation = mapper.Map<Entities.Models.EmployeeEducation>(request);
                    _employeeEducation.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeEducation.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeEducation>().Add(_employeeEducation);
                    SaveChanges();
                }
                else
                {
                    var _employeeEducation = mapper.Map<Entities.Models.EmployeeEducation>(request);
                    _employeeEducation.CreatedById = employeeEducation.CreatedById;
                    _employeeEducation.CreatedDate = employeeEducation.CreatedDate;
                    _employeeEducation.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeEducation.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeEducation>().Update(_employeeEducation);
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