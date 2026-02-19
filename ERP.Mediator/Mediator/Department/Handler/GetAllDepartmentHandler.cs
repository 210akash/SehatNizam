using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class GetAllDepartmentHandler : IRequestHandler<GetAllDepartmentQuery, Tuple<IEnumerable<GetDepartment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetDepartment>, long>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Department, bool>> predicate = x => x.IsActive == true
            && x.CompanyId == sessionProvider.Session.CompanyId;

            Expression<Func<Entities.Models.Department, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            Expression<Func<Entities.Models.Department, object>> OrderBy = null;
            Expression<Func<Entities.Models.Department, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Department>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var department = mapper.Map<IEnumerable<GetDepartment>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetDepartment>, long>(department, entity.Item2);
        }
    }
}
