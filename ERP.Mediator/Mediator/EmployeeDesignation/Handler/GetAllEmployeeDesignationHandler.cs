using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeDesignation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Handler
{
    public class GetAllEmployeeDesignationHandler : IRequestHandler<GetAllEmployeeDesignationQuery, Tuple<IEnumerable<GetEmployeeDesignation>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeDesignationHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeDesignation>, long>> Handle(GetAllEmployeeDesignationQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeDesignation, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeDesignation, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeDesignation, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeDesignation, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeDesignation = mapper.Map<IEnumerable<GetEmployeeDesignation>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeDesignation>, long>(employeeDesignation, entity.Item2);
        }
    }
}
