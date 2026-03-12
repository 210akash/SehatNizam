using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeWorkSiteType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeWorkSiteType.Handler
{
    public class SaveEmployeeWorkSiteTypeHandler : IRequestHandler<SaveEmployeeWorkSiteTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeWorkSiteTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeWorkSiteTypeCommand, long>.Handle(SaveEmployeeWorkSiteTypeCommand request, CancellationToken cancellationToken)
        {
            var employeeDesignation = await unitOfWork.Repository<Entities.Models.EmployeeWorkSiteType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeWorkSiteType>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeDesignation == null)
                {
                    var _employeeDesignation = mapper.Map<Entities.Models.EmployeeWorkSiteType>(request);
                    _employeeDesignation.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDesignation.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeWorkSiteType>().Add(_employeeDesignation);
                    SaveChanges();
                }
                else
                {
                    var _employeeDesignation = mapper.Map<Entities.Models.EmployeeWorkSiteType>(request);
                    _employeeDesignation.CreatedById = employeeDesignation.CreatedById;
                    _employeeDesignation.CreatedDate = employeeDesignation.CreatedDate;
                    _employeeDesignation.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeDesignation.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeWorkSiteType>().Update(_employeeDesignation);
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