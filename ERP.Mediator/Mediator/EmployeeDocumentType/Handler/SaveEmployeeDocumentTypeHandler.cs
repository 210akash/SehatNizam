using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDocumentType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Handler
{
    public class SaveEmployeeDocumentTypeHandler : IRequestHandler<SaveEmployeeDocumentTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeDocumentTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeDocumentTypeCommand, long>.Handle(SaveEmployeeDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var employeeDocumentType = await unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeDocumentType == null)
                {
                    var _employeeDocumentType = mapper.Map<Entities.Models.EmployeeDocumentType>(request);
                    _employeeDocumentType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDocumentType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().Add(_employeeDocumentType);
                    SaveChanges();
                }
                else
                {
                    var _employeeDocumentType = mapper.Map<Entities.Models.EmployeeDocumentType>(request);
                    _employeeDocumentType.CreatedById = employeeDocumentType.CreatedById;
                    _employeeDocumentType.CreatedDate = employeeDocumentType.CreatedDate;
                    _employeeDocumentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDocumentType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeDocumentType>().Update(_employeeDocumentType);
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