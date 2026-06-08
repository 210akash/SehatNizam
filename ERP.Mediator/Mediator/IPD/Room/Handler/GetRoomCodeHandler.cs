using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Room.Query;

namespace ERP.Mediator.Mediator.IPD.Room.Handler
{
    public class GetRoomCodeHandler : IRequestHandler<GetRoomCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetRoomCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetRoomCodeQuery request, CancellationToken cancellationToken)
        {
            var Ward = await unitOfWork.Repository<Entities.Models.Ward>().GetFirstAsNoTrackingAsync(x => x.Id == request.WardId && x.IsActive == true);
            string _RoomCode = "";
            if (await unitOfWork.Repository<Entities.Models.Room>().GetExistsAsync(y =>  y.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.WardId == request.WardId && y.Id != request.Id && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.Room>, IOrderedQueryable<Entities.Models.Room>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var RoomCode = await unitOfWork.Repository<Entities.Models.Room>().GetOneAsync(y => y.IsActive == true && y.WardId == request.WardId && y.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(RoomCode.Code.TakeLast(2).ToArray())) + 1;
                _RoomCode = No.ToString().PadLeft(2, '0');
            }
            else
                _RoomCode = "01";
          
            return Ward.Code + _RoomCode;
        }
    }
}
