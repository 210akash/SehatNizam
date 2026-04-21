using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Handler
{
    public class GetAllSalaryHeadsHandler : IRequestHandler<GetAllSalaryHeadsQuery, IEnumerable<GetSalaryHead>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSalaryHeadsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetSalaryHead>> Handle(GetAllSalaryHeadsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.SalaryHead, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false;

            var salaryHeads = await unitOfWork.Repository<Entities.Models.SalaryHead>().FindAsync(predicate);

            return mapper.Map<IEnumerable<GetSalaryHead>>(salaryHeads);
        }
    }
}
