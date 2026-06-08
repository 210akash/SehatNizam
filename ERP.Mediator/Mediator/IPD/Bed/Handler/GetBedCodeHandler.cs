using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Bed.Query;

namespace ERP.Mediator.Mediator.IPD.Bed.Handler
{
    public class GetBedCodeHandler : IRequestHandler<GetBedCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetBedCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetBedCodeQuery request, CancellationToken cancellationToken)
        {
            var Room = await unitOfWork.Repository<Entities.Models.Room>().GetFirstAsNoTrackingAsync(x => x.Id == request.RoomId && x.IsActive == true);
            string _BedCode = "";
            if (await unitOfWork.Repository<Entities.Models.Bed>().GetExistsAsync(y =>  y.IsActive == true && y.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.RoomId == request.RoomId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.Bed>, IOrderedQueryable<Entities.Models.Bed>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var BedCode = await unitOfWork.Repository<Entities.Models.Bed>().GetOneAsync(y => y.IsActive == true && y.RoomId == request.RoomId && y.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(BedCode.Code.TakeLast(2).ToArray())) + 1;
                _BedCode = No.ToString().PadLeft(3, '0');
            }
            else
                _BedCode = "001";
          
            return Room.Code + _BedCode;
        }
    }
}
