using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Handler
{
    public class GetAllSalaryHeadsHandler : IRequestHandler<GetAllSalaryHeadsQuery, Tuple<IEnumerable<GetSalaryHead>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllSalaryHeadsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetSalaryHead>, long>> Handle(GetAllSalaryHeadsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.SalaryHead, bool>> predicate = x => x.IsActive == true && x.Name.ToLower().Contains(request.Name.ToLower());    

            Expression<Func<Entities.Models.SalaryHead, object>> OrderBy = null;
            Expression<Func<Entities.Models.SalaryHead, object>> OrderByDesc = x => x.CreatedDate;
            var entity = unitOfWork.Repository<Entities.Models.SalaryHead>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, null);
            var salaryHeads = mapper.Map<IEnumerable<GetSalaryHead>>(entity.Item1).ToList();
            return new Tuple<IEnumerable<GetSalaryHead>, long>(salaryHeads, entity.Item2);
        }
    }
}
