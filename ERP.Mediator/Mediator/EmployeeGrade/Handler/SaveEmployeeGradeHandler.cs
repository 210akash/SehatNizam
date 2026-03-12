using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeGrade.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Handler
{
    public class SaveEmployeeGradeHandler : IRequestHandler<SaveEmployeeGradeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeGradeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeGradeCommand, long>.Handle(SaveEmployeeGradeCommand request, CancellationToken cancellationToken)
        {
            var employeeGrade = await unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeGrade == null)
                {
                    var _employeeGrade = mapper.Map<Entities.Models.EmployeeGrade>(request);
                    _employeeGrade.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeGrade.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeGrade>().Add(_employeeGrade);
                    SaveChanges();
                }
                else
                {
                    var _employeeGrade = mapper.Map<Entities.Models.EmployeeGrade>(request);
                    _employeeGrade.CreatedById = employeeGrade.CreatedById;
                    _employeeGrade.CreatedDate = employeeGrade.CreatedDate;
                    _employeeGrade.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeGrade.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeGrade>().Update(_employeeGrade);
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