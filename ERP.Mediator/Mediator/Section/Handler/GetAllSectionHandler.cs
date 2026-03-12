using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Section.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Handler
{
    public class GetAllSectionHandler : IRequestHandler<GetAllSectionQuery, Tuple<IEnumerable<GetSection>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllSectionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetSection>, long>> Handle(GetAllSectionQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Section, bool>> predicate = x => x.IsActive == true
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()))
            ;

            Expression<Func<Entities.Models.Section, object>>[] includes = {
                x => x.Row,
                x => x.Row.Rack
            };

            Expression<Func<Entities.Models.Section, object>> OrderBy = null;
            Expression<Func<Entities.Models.Section, object>> OrderByDescending = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Section>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDescending, null, includes);
            var Section = mapper.Map<IEnumerable<GetSection>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetSection>, long>(Section, entity.Item2);
        }
    }
}
