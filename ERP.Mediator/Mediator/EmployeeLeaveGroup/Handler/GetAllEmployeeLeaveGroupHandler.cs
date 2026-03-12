using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class GetAllEmployeeLeaveGroupHandler : IRequestHandler<GetAllEmployeeLeaveGroupQuery, Tuple<IEnumerable<GetEmployeeLeaveGroup>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeLeaveGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeLeaveGroup>, long>> Handle(GetAllEmployeeLeaveGroupQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeLeaveGroup, bool>> predicate = x => x.IsActive == true;
            Expression<Func<Entities.Models.EmployeeLeaveGroup, object>>[] includes = {
                x => x.CreatedBy,
                x => x.EmployeeGroupLeaveType.Where(x => x.IsActive == true)
            };

            List<string> thenInclude = new()
            {
                "EmployeeGroupLeaveType.HRYear",
                "EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail",
                "EmployeeGroupLeaveType.EmployeeGroupLeaveTypeDetail.EmployeeLeaveType"
            };

            Expression<Func<Entities.Models.EmployeeLeaveGroup, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeLeaveGroup, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);

            var employeeLeaveGroup = mapper.Map<IEnumerable<GetEmployeeLeaveGroup>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeLeaveGroup>, long>(employeeLeaveGroup, entity.Item2);
        }
    }
}
