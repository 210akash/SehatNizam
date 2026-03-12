using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeGrade.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Handler
{
    public class GetAllEmployeeGradeHandler : IRequestHandler<GetAllEmployeeGradeQuery, Tuple<IEnumerable<GetEmployeeGrade>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeGradeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeGrade>, long>> Handle(GetAllEmployeeGradeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeGrade, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeGrade, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeGrade, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeGrade, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeGrade = mapper.Map<IEnumerable<GetEmployeeGrade>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeGrade>, long>(employeeGrade, entity.Item2);
        }
    }
}
