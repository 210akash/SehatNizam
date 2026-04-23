using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Handler
{
    public class GetAllSalaryTaxSlabHandler : IRequestHandler<GetAllSalaryTaxSlabQuery, Tuple<IEnumerable<GetSalaryTaxSlab>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllSalaryTaxSlabHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSalaryTaxSlab>, long>> Handle(GetAllSalaryTaxSlabQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.SalaryTaxSlab, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.SalaryTaxSlab, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.SalaryTaxSlab, object>> OrderBy = null;
            Expression<Func<Entities.Models.SalaryTaxSlab, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var salarytaxslab = mapper.Map<IEnumerable<GetSalaryTaxSlab>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetSalaryTaxSlab>, long>(salarytaxslab, entity.Item2);
        }
    }
}
