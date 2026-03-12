using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class GetAllCompanyHandler : IRequestHandler<GetAllCompanyQuery, Tuple<IEnumerable<GetCompany>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetCompany>, long>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Company, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Company, object>>[] includes = {
                x => x.CreatedBy
            };

            Expression<Func<Entities.Models.Company, object>> OrderBy = null;
            Expression<Func<Entities.Models.Company, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Company>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var company = mapper.Map<IEnumerable<GetCompany>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetCompany>, long>(company, entity.Item2);
        }
    }
}
