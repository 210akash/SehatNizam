using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeShift.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Handler
{
    public class GetAllEmployeeShiftHandler : IRequestHandler<GetAllEmployeeShiftQuery, Tuple<IEnumerable<GetEmployeeShift>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeShiftHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeShift>, long>> Handle(GetAllEmployeeShiftQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeShift, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeShift, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeShift, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeShift, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeShift>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeShift = mapper.Map<IEnumerable<GetEmployeeShift>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeShift>, long>(employeeShift, entity.Item2);
        }
    }
}
