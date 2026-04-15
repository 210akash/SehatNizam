using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Query
{
    public class GetAllCandidateEvaluationCategoryQuery : IRequest<Tuple<IEnumerable<GetCandidateEvaluationCategory>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}