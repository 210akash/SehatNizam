using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeOvertimeRate.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeOvertimeRate.Handler
{
    public class GetAllEmployeeOvertimeRateHandler : IRequestHandler<GetAllEmployeeOvertimeRateQuery, Tuple<IEnumerable<GetEmployeeOvertimeRate>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeOvertimeRateHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeOvertimeRate>, long>> Handle(GetAllEmployeeOvertimeRateQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeOvertimeRate, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeOvertimeRate, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeOvertimeRate, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeOvertimeRate, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeOvertimeRate>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var EmployeeOvertimeRate = mapper.Map<IEnumerable<GetEmployeeOvertimeRate>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeOvertimeRate>, long>(EmployeeOvertimeRate, entity.Item2);
        }
    }
}
