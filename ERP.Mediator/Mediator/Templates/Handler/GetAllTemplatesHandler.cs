using AutoMapper;
using MediatR;
using ERP.BusinessModels.ParameterVM;
using ERP.Mediator.Mediator.Templates.Query;
using ERP.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace ERP.Mediator.Mediator.Templates.Handler
{
    public class GetAllTemplatesHandler : IRequestHandler<GetAllTemplatesQuery, Tuple<IEnumerable<GetTemplates>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllTemplatesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetTemplates>, long>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Templates, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Templates, object>>[] includes = {
            };

            Expression<Func<Entities.Models.Templates, object>> OrderBy = null;
            Expression<Func<Entities.Models.Templates, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Templates>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var templates = mapper.Map<IEnumerable<GetTemplates>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetTemplates>, long>(templates, entity.Item2);
        }
    }
}
