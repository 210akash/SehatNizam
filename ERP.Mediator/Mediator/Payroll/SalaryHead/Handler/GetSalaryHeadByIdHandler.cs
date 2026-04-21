using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Handler
{
    public class GetSalaryHeadByIdHandler : IRequestHandler<GetSalaryHeadByIdQuery, GetSalaryHead>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetSalaryHeadByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetSalaryHead> Handle(GetSalaryHeadByIdQuery request, CancellationToken cancellationToken)
        {
            var salaryHead = await unitOfWork.Repository<Entities.Models.SalaryHead>().FindAsync(x=>x.Id == request.Id);
            if (salaryHead == null || salaryHead.IsDelete)
            {
                return null;
            }

            return mapper.Map<GetSalaryHead>(salaryHead);
        }
    }
}
