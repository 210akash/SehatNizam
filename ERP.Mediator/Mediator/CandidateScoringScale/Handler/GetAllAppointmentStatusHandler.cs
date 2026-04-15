using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.CandidateScoringScale.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateScoringScale.Handler
{
    public class GetAllCandidateScoringScaleStatusHandler : IRequestHandler<GetAllCandidateScoringScaleQuery, List<GetCandidateScoringScale>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllCandidateScoringScaleStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCandidateScoringScale>> Handle(GetAllCandidateScoringScaleQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.CandidateScoringScale>().GetAllAsync();
            var order = mapper.Map<IEnumerable<GetCandidateScoringScale>>(entity).ToList();
            return order;
        }
    }
}
