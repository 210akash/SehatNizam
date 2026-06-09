using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.Bed.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Handler
{
    public class GetBedByRoomHandler : IRequestHandler<GetBedByRoomQuery, List<GetBed>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBedByRoomHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetBed>> Handle(GetBedByRoomQuery request, CancellationToken cancellationToken)
        {
            var Bed = await unitOfWork.Repository<Entities.Models.Bed>().GetAsync(y => y.IsActive == true && y.RoomId == request.RoomId && (!request.Vacant || !y.IsOccupied == request.Vacant));
            var _Bed = mapper.Map<List<GetBed>>(Bed);
            return _Bed;
        }
    }
}
