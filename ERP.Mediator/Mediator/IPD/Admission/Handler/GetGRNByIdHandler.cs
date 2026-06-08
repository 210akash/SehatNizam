using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetGRNByIdHandler : IRequestHandler<GetGRNByIdQuery, GetGRN>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetGRNByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetGRN> Handle(GetGRNByIdQuery request, CancellationToken cancellationToken)
        {
            var GRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _GRN = mapper.Map<GetGRN>(GRN);
            return _GRN;
        }
    }
}
