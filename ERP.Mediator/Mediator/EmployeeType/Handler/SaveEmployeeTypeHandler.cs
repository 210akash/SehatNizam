using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Handler
{
    public class SaveEmployeeTypeHandler : IRequestHandler<SaveEmployeeTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeTypeCommand, long>.Handle(SaveEmployeeTypeCommand request, CancellationToken cancellationToken)
        {
            var employeeType = await unitOfWork.Repository<Entities.Models.EmployeeType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeType>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeType == null)
                {
                    var _employeeType = mapper.Map<Entities.Models.EmployeeType>(request);
                    _employeeType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeType>().Add(_employeeType);
                    SaveChanges();
                }
                else
                {
                    var _employeeType = mapper.Map<Entities.Models.EmployeeType>(request);
                    _employeeType.CreatedById = employeeType.CreatedById;
                    _employeeType.CreatedDate = employeeType.CreatedDate;
                    _employeeType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeType>().Update(_employeeType);
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