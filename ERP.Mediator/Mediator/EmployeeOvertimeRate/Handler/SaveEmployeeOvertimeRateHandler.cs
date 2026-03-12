using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeOvertimeRate.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Handler
{
    public class SaveEmployeeOvertimeRateHandler : IRequestHandler<SaveEmployeeOvertimeRateCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeOvertimeRateHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeOvertimeRateCommand, long>.Handle(SaveEmployeeOvertimeRateCommand request, CancellationToken cancellationToken)
        {
            var EmployeeOvertimeRate = await unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (EmployeeOvertimeRate == null)
                {
                    var _EmployeeOvertimeRate = mapper.Map<Entities.Models.EmployeeOvertimeRate>(request);
                    _EmployeeOvertimeRate.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _EmployeeOvertimeRate.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().Add(_EmployeeOvertimeRate);
                    SaveChanges();
                }
                else
                {
                    var _EmployeeOvertimeRate = mapper.Map<Entities.Models.EmployeeOvertimeRate>(request);
                    _EmployeeOvertimeRate.CreatedById = EmployeeOvertimeRate.CreatedById;
                    _EmployeeOvertimeRate.CreatedDate = EmployeeOvertimeRate.CreatedDate;
                    _EmployeeOvertimeRate.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _EmployeeOvertimeRate.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().Update(_EmployeeOvertimeRate);
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