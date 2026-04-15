using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.CandidateEvaluationCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Handler
{
    public class GetAllCandidateEvaluationCategoryHandler : IRequestHandler<GetAllCandidateEvaluationCategoryQuery, Tuple<IEnumerable<GetCandidateEvaluationCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllCandidateEvaluationCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetCandidateEvaluationCategory>, long>> Handle(GetAllCandidateEvaluationCategoryQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.CandidateEvaluationCategory, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.CandidateEvaluationCategory, object>>[] includes = {
                x => x.CreatedBy
            };

            Expression<Func<Entities.Models.CandidateEvaluationCategory, object>> OrderBy = null;
            Expression<Func<Entities.Models.CandidateEvaluationCategory, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var CandidateEvaluationCategory = mapper.Map<IEnumerable<GetCandidateEvaluationCategory>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetCandidateEvaluationCategory>, long>(CandidateEvaluationCategory, entity.Item2);
        }
    }
}
