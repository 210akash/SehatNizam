using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.Payroll.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Handler
{
    public class GetAllPayrollsHandler : IRequestHandler<GetAllPayrollsQuery, IEnumerable<GetPayroll>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPayrollsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetPayroll>> Handle(GetAllPayrollsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Payroll, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false;

            if (request.Month.HasValue)
            {
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.Month == request.Month.Value;
            }

            if (request.Year.HasValue)
            {
                var currentPredicate = predicate;
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.Year == request.Year.Value;
            }

            if (request.Month.HasValue && request.Year.HasValue)
            {
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.Month == request.Month.Value &&
                    x.Year == request.Year.Value;
            }

            var payrolls = await unitOfWork.Repository<Entities.Models.Payroll>().FindAllAsync(predicate);

            var result = mapper.Map<IEnumerable<GetPayroll>>(payrolls.ToList());

            // Order by year desc, then month desc
            return result.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
        }
    }
}
