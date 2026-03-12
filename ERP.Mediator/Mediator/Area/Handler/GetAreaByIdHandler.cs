using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Area.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Handler
{
    public class GetAreaByIdHandler : IRequestHandler<GetAreaByIdQuery, GetArea>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAreaByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetArea> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken)
        {
            var area = await unitOfWork.Repository<Entities.Models.Area>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _area = mapper.Map<GetArea>(area);
            return _area;
        }
    }
}
