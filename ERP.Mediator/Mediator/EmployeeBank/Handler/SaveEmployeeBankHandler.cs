using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeBank.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Handler
{
    public class SaveEmployeeBankHandler : IRequestHandler<SaveEmployeeBankCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeBankHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveEmployeeBankCommand, long>.Handle(SaveEmployeeBankCommand request, CancellationToken cancellationToken)
        {
            var employeeBank = await unitOfWork.Repository<Entities.Models.EmployeeBank>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.EmployeeBank>().GetAsync(x => x.BankName.ToLower().Trim() == request.BankName.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (employeeBank == null)
                {
                    var _employeeBank = mapper.Map<Entities.Models.EmployeeBank>(request);
                    _employeeBank.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _employeeBank.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeBank>().Add(_employeeBank);
                    SaveChanges();
                }
                else
                {
                    var _employeeBank = mapper.Map<Entities.Models.EmployeeBank>(request);
                    _employeeBank.CreatedById = employeeBank.CreatedById;
                    _employeeBank.CreatedDate = employeeBank.CreatedDate;
                    _employeeBank.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _employeeBank.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.EmployeeBank>().Update(_employeeBank);
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