using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Department.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class SaveDepartmentHandler : IRequestHandler<SaveDepartmentCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveDepartmentHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveDepartmentCommand, long>.Handle(SaveDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Department>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == request.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (department == null)
                {
                    var _department = mapper.Map<Entities.Models.Department>(request);
                    _department.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _department.CompanyId = sessionProvider.Session.CompanyId;
                    _department.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Department>().Add(_department);
                    SaveChanges();
                }
                else
                {
                    var _department = mapper.Map<Entities.Models.Department>(request);
                    _department.CreatedById = department.CreatedById;
                    _department.CreatedDate = department.CreatedDate;
                    _department.CompanyId = department.CompanyId;
                    _department.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _department.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Department>().Update(_department);
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