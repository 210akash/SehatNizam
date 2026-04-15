using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.CandidateScoringScale.Query
{
    public class GetAllCandidateScoringScaleQuery : IRequest<List<GetCandidateScoringScale>>
    {

    }
}