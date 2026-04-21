using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Payroll.Payroll.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Handler
{
    public class GetPayrollByIdHandler : IRequestHandler<GetPayrollByIdQuery, GetPayroll>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPayrollByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetPayroll> Handle(GetPayrollByIdQuery request, CancellationToken cancellationToken)
        {
            var payroll = await unitOfWork.Repository<Entities.Models.Payroll>().FindAllAsync(x => x.Id == request.Id);
            if (payroll == null)
            {
                return null;
            }

            var result = mapper.Map<GetPayroll>(payroll);

            // Load payroll details
            var details = await unitOfWork.Repository<Entities.Models.PayrollDetail>()
                .FindAsync(x => x.PayrollId == request.Id && !x.IsDelete);

            result.PayrollDetails = mapper.Map<List<GetPayrollDetail>>(details);

            // Fill employee names
            //foreach (var detail in result.PayrollDetails)
            //{
            //    var employee = await unitOfWork.Repository<Entities.Models.Employee>().GetByIdAsync(detail.EmployeeId);
            //    if (employee != null)
            //    {
            //        detail.EmployeeName = employee.FullName;
            //    }
            //}

            return result;
        }
    }
}
