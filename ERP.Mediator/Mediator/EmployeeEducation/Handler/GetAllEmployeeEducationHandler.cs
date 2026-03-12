using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeEducation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Handler
{
    public class GetAllEmployeeEducationHandler : IRequestHandler<GetAllEmployeeEducationQuery, Tuple<IEnumerable<GetEmployeeEducation>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeEducationHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeEducation>, long>> Handle(GetAllEmployeeEducationQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeEducation, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeEducation, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeEducation, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeEducation, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeEducation = mapper.Map<IEnumerable<GetEmployeeEducation>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeEducation>, long>(employeeEducation, entity.Item2);
        }
    }
}
