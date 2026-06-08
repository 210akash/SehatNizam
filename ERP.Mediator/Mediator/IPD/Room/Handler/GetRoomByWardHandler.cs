using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.Room.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Handler
{
    public class GetRoomByWardHandler : IRequestHandler<GetRoomByWardQuery, List<GetRoom>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRoomByWardHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoom>> Handle(GetRoomByWardQuery request, CancellationToken cancellationToken)
        {
            var Room = await unitOfWork.Repository<Entities.Models.Room>().GetAsync(y => y.IsActive == true && y.WardId == request.WardId);
            var _Room = mapper.Map<List<GetRoom>>(Room);
            return _Room;
        }
    }
}
